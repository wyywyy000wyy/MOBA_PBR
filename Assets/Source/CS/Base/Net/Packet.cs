// Galaxy Network

using XLua;
using System;
using System.Net;
using UnityEngine;
using System.Collections.Generic;
[LuaCallCSharp]
public class Packet
{
    public struct PacketHeader
    {
        public UInt32 opCode;
        public int length;
    };
    public PacketHeader header;
    public byte[] body = null;


    public Packet(UInt32 opCode, byte[] data)
    {
        header.opCode = opCode;
        header.length = data.Length;
        body = data;
    }
    public Packet(UInt32 opCode, string data)
    {
        header.opCode = opCode;
        header.length = data.Length;
        body = new byte[data.Length];
        System.Buffer.BlockCopy(data.ToCharArray(), 0, body, 0, data.Length);
    }


    public Packet(UInt32 opCode = 0)
    {
        header.opCode = opCode;
        header.length = 0;
    }

    
}

