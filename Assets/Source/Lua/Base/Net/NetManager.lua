

local pb = require 'pb'
local protoc = require 'protoc'
local proto = require("Base/Net/Proto")
local WarNet 
require("Base/Net/MsgHandler")

NetManager = {

}





function NetManager:Init()
    WarNet = U.WarNet.instance
    WarNet.onNetEvent = function (NetEventType)
        ELOG("onNetEvent", NetEventType)
        NetManager:SendMsg(Protocol.C2S_Login, { username = "tt", password = "7526" } )
    end

    -- WarNet = U.WarNet.instance
    WarNet.onReceivePacketEvent = function (packet)
        ELOG("onReceivePacketEvent", packet)
        local msgType = MsgHandler.pro[packet.header.opCode]
        local msg = pb.decode(msgType, packet.body)
        ELOG("onReceivePacketEvent msg", DT(msg))
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
    ELOG("SendMsg", msgType)
    ELOG("bytes", bytes and #bytes,type(bytes), bytes)
    local packet = U.Packet(opCode, bytes)
    WarNet:Send(packet)
end
