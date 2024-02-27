PROTO = PROTO or {}
local PROTO = PROTO
local object_proto = class("object_proto", function(self)
end)

function object_proto.load_proto(name)
    local obj_proto = PROTO[name]
    if obj_proto then
        return obj_proto
    end
end