
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