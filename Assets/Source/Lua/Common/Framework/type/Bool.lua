T.Bool = T.Bool or {}
Bool = T.Bool
local Bool = Bool
Bool.name = "Bool"
Bool.v = false
Bool.__tostring = function(self)
    return self.v
end
Bool.ctor = function(init_value)
    return init_value or false
end

local BoolMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, Bool)
        return init_value or false
    end
}

setmetatable(Bool, BoolMt)