#define NETWORK_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using pb = global::Google.Protobuf;

public class ConnectionProxy
{
    public Action<Packet> onReceiveEvent;
    public Action onConnectEvent;
    public Action onConnectErrorEvent;
    public Action onCloseEvent;
    public Action<byte[], int> decodeFunc;
    public Action<byte[], int> encodeFunc;
    AutoResetEvent sendThreadEvent = new AutoResetEvent(false);
    public enum WorkingStatus
    {
        None,
        Connecting,
        Working,
        Close,
    }

    bool isShaked = false;

    public WorkingStatus status = WorkingStatus.None;

    string host;
    int port;
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    Thread sendThread;

    private ArrayList sendQueue;
    public void Connect(string host, int port)
    {

        this.host = host;
        this.port = port;
        status = WorkingStatus.Connecting;
        Debug.LogFormat("[WarNet.Connect] Connect(" + host + ":" + port + ")");
        try
        {
            if (client != null)
                client.Close();
            IPAddress[] ips;
            bool ipv6 = false;

            ips = Dns.GetHostAddresses(host);
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ipv6 = true;
                    break;
                }
            }
            if (ipv6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
                Debug.Log("[WarNet.Connect]client.BeginConnect ipv6 async...");
            }
            else
            {
                client = new TcpClient();
                Debug.Log("[WarNet.Connect]client.BeginConnect async...");
            }
            client.NoDelay = true;
            client.BeginConnect(ips, port, new AsyncCallback(OnConnected), client);
        }
        catch (Exception e)
        {
            HandleConnectError(e.ToString());
        }
    }

    public void Send(Packet p)
    {
        if (sendQueue != null)
        {
            sendQueue.Add(p);
            sendThreadEvent.Set();
        }
    }

    public void Close()
    {
        if (stream != null)
        {
            stream.Close();
            //stream.Dispose();
        }
        if (sendThreadEvent != null)
            sendThreadEvent.Set();

        stream = null;

        if (client != null)
            client.Close();
        client = null;

        switch (status)
        {
            case WorkingStatus.Connecting:
                status = WorkingStatus.Close;
                if (onConnectErrorEvent != null)
                    onConnectErrorEvent();
                break;
            case WorkingStatus.Working:
                status = WorkingStatus.Close;
                if (onCloseEvent != null)
                    onCloseEvent();
                break;
            case WorkingStatus.None:
            case WorkingStatus.Close:
                return;
            default:
                break;
        }
    }

    public void SetClient(TcpClient client)
    {
        this.client = client;
        stream = client.GetStream();
        status = WorkingStatus.Working;

        try
        {
            if (receiveThread != null)
                receiveThread.Abort();
            if (sendThread != null)
                sendThread.Abort();
        }
        catch
        {
        }


        receiveThread = new Thread(new ThreadStart(ReceiveHandler));
        receiveThread.Start();

        sendQueue = ArrayList.Synchronized(new ArrayList());
        sendThread = new Thread(new ThreadStart(SendHandler));
        sendThread.Start();

        if (onConnectEvent != null)
            onConnectEvent();
    }

    private void OnConnected(IAsyncResult rs)
    {
        try
        {
            client.EndConnect(rs);
            SetClient(client);

            Debug.Log("[WarNet.Connect]client Connected...");
        }
        catch (Exception e)
        {
            HandleConnectError(e.ToString());
        }
    }

    void ReceiveHandler()
    {
        byte[] header = new byte[8];
        byte[] buffer = new byte[204800];

        int len = 0;
        int nread = 0;
        int size_int = 8;// sizeof(int);


        //nread = 0;
        //while (nread < 8 && client.Connected)
        //{
        //    try
        //    {
        //        nread += stream.Read(buffer, nread, 8 - nread);
        //    }
        //    catch (Exception e)
        //    {
        //        HandleRunningError(string.Format("[NET] stream.Read, catch:{0}", e));
        //        return;
        //    }
        //}
        //if (nread != 8)
        //{
        //    HandleRunningError(string.Format("[NET]  stream.Read, Error nread({0}) != size_int({1}) ", nread, 8));
        //    return;
        //}
        //uint opCode = BitConverter.ToUInt32(buffer, 0);
        //uint length = BitConverter.ToUInt32(buffer, 4);
        //if (a1 == 8)
        //{
        //    if (a2 != 0)
        //    {
        //        try
        //        {
        //            isShaked = true;
        //            Debug.Log("[WarNet.Connect]client Shake Finish...");
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogError(e.ToString());
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    return;
        //}
        //if (onConnectEvent != null)
        //    onConnectEvent();

        while (status == WorkingStatus.Working)
        {
            nread = 0;
            while (nread < size_int && client.Connected)
            {
                try
                {
                    nread += stream.Read(header, nread, size_int - nread);
                }
                catch (Exception e)
                {
                    HandleRunningError(string.Format("[NET] stream.Read, catch:{0}", e));
                    return;
                }
            }
            if (nread != size_int)
            {
                HandleRunningError(string.Format("[NET]  stream.Read, Error nread({0}) != size_int({1}) ", nread, size_int));
                return;
            }

            uint opCode = BitConverter.ToUInt32(header, 0);
            len = BitConverter.ToInt32(header, 4);

            //// 解密
            //if (decodeFunc != null)
            //    decodeFunc(header, 4);

            //len = BitConverter.ToInt32(header, 0) - 4;
            // len = IPAddress.NetworkToHostOrder(len);

            if (buffer.Length < len)
            {
                //if (len >= 256 * 1024)       // 大于256K，认为他是异常的，输出一些日志出来
                //{
                //    HandleRunningError(string.Format("WarNet.Receive > 256K, Maybe it's Exception, Len = {0}", len));
                //    return;
                //}
                buffer = new byte[len];
            }
            nread = 0;
            while (nread < len && client.Connected)
            {
                try
                {
                    nread += stream.Read(buffer, nread, len - nread);
                }
                catch (Exception e)
                {
                    HandleRunningError(string.Format("[NET] stream.Read, Error {0} ", e));
                    return;
                }
            }
            if (nread != len)
            {
                HandleRunningError(string.Format("[NET] stream.Read, Error nread({0}) != len({1}) ", nread, len));
                return;
            }

            // 解密
            if (decodeFunc != null)
                decodeFunc(buffer, len);

            Packet packet = new Packet(opCode, buffer);
            // 解密
            //pb::CodedInputStream input = new pb.CodedInputStream(buffer);
            //Packet packet = new Packet();
            ////packet.Set(len, buffer, 0);
            //packet.MergeFrom(input);


            if (onReceiveEvent != null)
                onReceiveEvent(packet);
            Thread.Sleep(1);
        }
    }

    void SendHandler()
    {
        while (status == WorkingStatus.Working)
        {
#if !UNITY_EDITOR
            sendThreadEvent.WaitOne();
#endif
            while (sendQueue.Count > 0)
            {
                Packet packet = (Packet)sendQueue[0];
                // Debug.Log(packet);
                //  packet.Finish();
                sendQueue.RemoveAt(0);
                try
                {
                    //if (encodeFunc != null)
                    //    encodeFunc(packet.GetBytes(), packet.Size);
                    //encode;


                    stream.Write(BitConverter.GetBytes(packet.header.opCode), 0, 4);
                    stream.Write(BitConverter.GetBytes(packet.header.length), 0, 4);
                    if(packet.header.length > 0)
                    {
                        stream.Write(packet.body, 0, packet.body.Length);
                    }
                }
                catch (Exception e)
                {
                    HandleRunningError(e.ToString());
                    return;
                }
            }
            Thread.Sleep(1);
        }
    }

    void HandleRunningError(string why)
    {
#if NETWORK_DEBUG
        Debug.LogFormat("[CONNECT ERROR]" + why);
#endif
        Close();
    }

    void HandleConnectError(string why)
    {
#if NETWORK_DEBUG
        Debug.LogFormat("[CONNECT ERROR]" + why);
#endif
        Close();
    }
}
