O = O or {}

local object_proto = T.object_proto

function object(name, ...)
    local obj_class = class(name, ...)
    local obj_proto = PROTO[name]
    if not obj_proto then
        obj_proto = object_proto.load_proto(name)
    end
    obj_class.__proto = obj_proto
    return obj_class
end