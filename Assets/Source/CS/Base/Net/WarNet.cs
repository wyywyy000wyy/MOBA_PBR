﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class WarNet : MonoBehaviour, ISington
{
    public static WarNet instance
    {
        get { return SingtonMng.GetMono<WarNet>(); }
    }

    public Action<NetEventType> onNetEvent;
    public Action<Msg> onReceivePacketEvent;

    ConnectionProxy cp;
    ArrayList eventQueue;
    ArrayList packetQueue;

    public void SingtonInit()
    {
        eventQueue = ArrayList.Synchronized(new ArrayList());
        packetQueue = ArrayList.Synchronized(new ArrayList());
    }

    public void Connect(string host, int port, bool isEncode)
    {
        Connect(new string[] { host }, port, isEncode);
    }

    public void Connect(string[] host, int port, bool isEncode)
    {
        Clear();
        cp = new ConnectionProxy();
        int index = 0;
        cp.onCloseEvent += () =>
        {
            eventQueue.Add(NetEventType.CLOSE);
        };
        cp.onConnectErrorEvent += () =>
        {
            index++;
            if (index < host.Length)
            {
                cp.Connect(host[index], port);
            }
            else
            {
                eventQueue.Add(NetEventType.CONNECT_ERROR);
            }
        };

        cp.onConnectEvent += () =>
        {
            if (!isEncode)
                eventQueue.Add(NetEventType.CONNECT);
        };

        cp.onReceiveEvent += (Msg p) =>
        {
            {
                packetQueue.Add(p);
            }
        };
        cp.Connect(host[index], port);
    }

    public void Send(Msg p)
    {
        if (cp != null)
            cp.Send(p);
    }


    void Update()
    {
        for (int i = 0; i < 3 && eventQueue.Count > 0; i++)
        {
            NetEventType e = (NetEventType)eventQueue[0];
            eventQueue.RemoveAt(0);
            if (onNetEvent != null)
                onNetEvent(e);
        }
        for (int i = 0; i < 3 && packetQueue.Count > 0; i++)
        {
            Msg p = (Msg)packetQueue[0];
            packetQueue.RemoveAt(0);
            if (onReceivePacketEvent != null)
                onReceivePacketEvent(p);
        }
    }

    public void Close()
    {
        if (cp != null)
            cp.Close();
    }

    void Clear()
    {
        if (cp != null)
        {
            cp.onCloseEvent = null;
            cp.onConnectErrorEvent = null;
            cp.onConnectEvent = null;
            cp.onReceiveEvent = null;
            cp.encodeFunc = null;
            cp.decodeFunc = null;
            cp.Close();
            cp = null;
        }
    }

    void OnDestroy()
    {
        Clear();
    }
}

[LuaCallCSharp]
public enum NetEventType
{
    CONNECT,
    CONNECT_ERROR,
    CLOSE,
}