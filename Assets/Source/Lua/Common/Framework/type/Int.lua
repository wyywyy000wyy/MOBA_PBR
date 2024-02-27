T.Int = T.Int or {}
Int = T.Int
local Int = Int
Int.name = "Int"
Int.v = 0
Int.__tostring = function(self)
    return self.v
end
Int.ctor = function(init_value)
    return init_value or 0
end

local IntMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, Int)
        return init_value or 0
    end
}

setmetatable(Int, IntMt)