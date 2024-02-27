local data_source = class("data_source", function(self)
    self._d = {}
end)

function data_source:set(key, value)
    local pre = self._d[key]
    if pre == value then
        return
    end
    self._d[key] = value

    if self._notify then
        self._notify(self, key, value, pre)
    end
end