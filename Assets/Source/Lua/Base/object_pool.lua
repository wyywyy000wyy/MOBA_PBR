local object_pool = class("object_pool",function(self, class_name, ...)
    self._objects = {}
    self._class_name = class_name
    self._args = {...}
end)

function object_pool:get_object()
    if #self._objects > 0 then
        return table.remove(self._objects)
    else
        return T[self._class_name](unpack(self._args))
    end
end

function object_pool:put_object(object)
    table.insert(self._objects, object)
end