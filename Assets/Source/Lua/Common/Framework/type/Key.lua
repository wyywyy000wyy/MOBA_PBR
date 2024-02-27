T.Key = T.Key or {}
Key = T.Key
local Key = Key
Key.name = "Key"
Key.v = 1
Key.__tostring = function(self)
    return self.v
end

Key.ctor = function(init_value)
    return init_value or 1
end

local KeyMt = {
    __call = function(_, init_value)
        return init_value or 1
    end
}

setmetatable(Key, KeyMt)