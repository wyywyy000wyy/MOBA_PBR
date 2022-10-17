
local Proto = [[
syntax = "proto3";

enum MsgType
{
	C2SLogin = 0;
	S2CLogin = 1;
}

message Msg
{
	MsgType type = 1;
	bytes data = 2;
}

message C2S_Login
{
	string username = 1;
	string password = 2;
	int32 tt = 3;
}

message S2C_Login
{
	int32 id = 1;
	int32 code = 2;
}

]]

return Proto