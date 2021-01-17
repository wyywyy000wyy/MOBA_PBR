// Galaxy Network

using XLua;
using System;
using System.Net;
using UnityEngine;

[LuaCallCSharp]
public class Packet
{
    public const int BufferSize = 4096;
    protected int size = 0;
    protected int type = 0;
    protected bool storing = false;
    protected byte[] data = null;
    protected int offset = 0;

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < size; i++)
        {
            s += data[i].ToString("x") + ",";
        }
        return s;
    }


    public Packet()
    {
        data = new byte[BufferSize];
        //offset = sizeof(int);
        storing = true;
    }

    public Packet(int packetType)
    {
        type = packetType;
        data = new byte[BufferSize];
        storing = true;
        offset = sizeof(int) * 2;	// skip size and type
        size = sizeof(int);
    }

    public int Size
    {			// type+data
        get { return size; }
        internal set { size = value; }
    }

    public int Type
    {
        get { return type; }
        internal set { type = value; }
    }

    public bool Storing
    {
        get { return storing; }
        internal set { storing = value; }
    }

    public void SetType(int newType)
    {
        type = newType;
    }

    public byte[] GetBytes()
    {
        return data;
    }

    public void Set(int packetSize, byte[] buffer, int start)
    {
        size = packetSize;
        data = new byte[size];
        Buffer.BlockCopy(buffer, start, data, 0, size);
        offset = start;
    }

    public char ReadChar()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        char value = (char)data[offset];
        offset++;
        return value;
    }

    public byte ReadByte()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        byte value = data[offset];
        offset++;
        return value;
    }

    public short ReadShort()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        short value = BitConverter.ToInt16(data, offset);
        offset += sizeof(short);
        return NetworkToHostOrder(value);
    }

    public ushort ReadUShort()
    {
        return (ushort)ReadShort();
    }

    public int ReadInt()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        int value = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);
        return NetworkToHostOrder(value);
    }

    public uint GetUint(int offset)
    {
        int value = BitConverter.ToInt32(data, offset);
        return (uint)NetworkToHostOrder(value);
    }

    public uint ReadUInt()
    {
        return (uint)ReadInt();
    }

    public float ReadFloat()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        float value = BitConverter.ToSingle(data, offset);
        offset += sizeof(int);
        return value;
    }

    public string ReadString()
    {
        ushort len = ReadUShort();
        if (offset + len > data.Length) throw (new Exception("read packet length error"));
        string str = System.Text.Encoding.UTF8.GetString(data, offset, len);
        offset += len;
        return str;
    }

    public byte[] ReadBytes(int len)
    {
        if (offset + len > data.Length) throw (new Exception("read packet length error"));
        byte[] buffer = new byte[len];
        Buffer.BlockCopy(data, offset, buffer, 0, len);
        offset += len;
        return buffer;
    }

    public byte[] ReadBlock()
    {
        if (offset >= data.Length) throw (new Exception("read packet length error"));
        int len = ReadInt();
        return ReadBytes(len);
    }


    void CheckSize(int ckl)
    {
        if (size + ckl >= data.Length)
        {
            byte[] tmp = new byte[data.Length * 2];
            Buffer.BlockCopy(data, 0, tmp, 0, data.Length);
            data = tmp;
        }
    }

    public void WriteChar(char value)
    {
        if (!storing) return;
        CheckSize(1);
        data[offset] = (byte)value;
        offset++;
        size++;
    }

    public void WriteByte(byte value)
    {
        if (!storing) return;
        CheckSize(1);
        data[offset] = value;
        offset++;
        size++;
    }

    public void WriteByteAt(byte value, int offset)
    {
        if (!storing) return;
        data[offset] = value;
    }

    public void WriteShort(short value)
    {
        if (!storing) return;
        CheckSize(2);
        value = HostToNetworkOrder(value);
        byte[] bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
        offset += bytes.Length;
        size += bytes.Length;
    }

    public void WriteUShort(ushort value)
    {
        WriteShort((short)value);
    }


    public void WriteInt(int value)
    {
        if (!storing) return;
        CheckSize(4);
        value = HostToNetworkOrder(value);
        byte[] bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
        offset += bytes.Length;
        size += bytes.Length;
    }

    public void WriteIntAt(int value, int offset)
    {
        if (!storing) return;
        value = HostToNetworkOrder(value);
        byte[] bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
    }

    public void WriteUInt(uint value)
    {
        WriteInt((int)value);
    }

    public void WriteFloat(float value)
    {
        if (!storing) return;
        CheckSize(4);
        byte[] bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
    }

    public void WriteString(string value)
    {
        if (!storing) return;
        if (value.Length > UInt16.MaxValue) return;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        ushort len = (ushort)bytes.Length;
        CheckSize(len + 2);
        WriteUShort(len);
        Buffer.BlockCopy(bytes, 0, data, offset, len);
        offset += len;
        size += len;
    }


    public void WriteBytes(byte[] buffer)
    {
        if (!storing) return;
        CheckSize(buffer.Length);
        int len = buffer.Length;
        Buffer.BlockCopy(buffer, 0, data, offset, len);
        offset += len;
        size += len;
    }

    public void WriteBlock(byte[] buffer)
    {
        WriteInt(buffer.Length);
        WriteBytes(buffer);
    }

    public void EncodeHeader()
    {
        // UnityEngine.Debug.Log("PackSize= " + Size);
        Size = HostToNetworkOrder(Size);
        byte[] bytes = BitConverter.GetBytes(Size);
        Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);
    }

    public void Finish()
    {
        if (!storing) return;

        // write packet size
        int value = HostToNetworkOrder(size);
        byte[] bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, 0, sizeof(int));

        // write packet type
        value = HostToNetworkOrder(type);
        bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, data, sizeof(int), sizeof(int));
    }

    public bool Equals(Packet other)
    {
        if (size == other.Size)
        {
            byte[] otherData = other.GetBytes();
            for (int i = 0; i < size; i++)
            {
                if (data[i] != otherData[i])
                    return false;
            }
            return true;
        }

        return false;
    }
    public static int HostToNetworkOrder(int i)
    {
        return i;
    }

    public static short HostToNetworkOrder(short i)
    {
        return i;
    }

    public static int NetworkToHostOrder(int i)
    {
        return i;
    }

    public static short NetworkToHostOrder(short i)
    {
        return i;
    }

    public int CalcSum(int begin_pos, int end_pos)
    {
        int sum = 0;
        for (int i = begin_pos; i < end_pos; i++)
        {
            sum += data[i];
        }
        sum &= 0x07FFF;
        return sum;
    }
}

