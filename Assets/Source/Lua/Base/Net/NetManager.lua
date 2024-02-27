

local pb = require 'pb'
local protoc = require 'protoc'
local proto = require("base/net/Proto")
local WarNet 
require("base/net/MsgProtocol")
require("base/net/MsgHandler")

NetManager = {

}





function NetManager:Init()
    WarNet = U.WarNet.instance
    WarNet.onNetEvent = function (NetEventType)
        NetManager:SendMsg(Protocol.C2S_Login, { username = "tt", password = "7526" } )
    end

    -- WarNet = U.WarNet.instance
    WarNet.onReceivePacketEvent = function (packet)
        local msgType = MsgHandler.pro[packet.header.opCode]
        local msg = pb.decode(msgType, packet.body)
    end

    protoc:load(proto)
    MsgHandler:Init()
end

function NetManager:Connect(host, port)
    WarNet:Connect(host, port)
end

function NetManager:SendMsg(opCode, msg)
    local msgType = MsgHandler.pro[opCode]
    local bytes = pb.encode(msgType, msg)
    local packet = U.Packet(opCode, bytes)
    ELOG("Send", DT(msg))

    ELOG("SendMsg", type(bytes), string.len(bytes))

    local dmsg = pb.decode(msgType, bytes)

    ELOG("dmsg", DT(dmsg))

    -- WarNet:Send(packet)
end
