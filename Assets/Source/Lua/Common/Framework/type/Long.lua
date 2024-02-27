T.Long = T.Long or {}
Long = T.Long
local Long = Long
Long.name = "Long"
Long.v = 0
Long.__tostring = function(self)
    return self.v
end
Long.ctor = function(init_value)
    return init_value or 0
end

local LongMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, Long)
        return init_value or 0
    end
}

setmetatable(Long, LongMt)