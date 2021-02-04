
Protocol = {
    C2S_Login = 0x00000000,
    S2C_Login = 0x00000001,
}

MsgHandler = {

}

function MsgHandler:Init()
    self.msg = {}
    self.pro = {}
    for msg, pro in pairs(Protocol) do
        self.msg[msg] = pro
        self.pro[pro] = msg
    end
end