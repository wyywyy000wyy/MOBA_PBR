T.Double = T.Double or {}
Double = T.Double
local Double = Double
Double.name = "Double"
Double.v = 0.0
Double.__tostring = function(self)
    return self.v
end
Double.ctor = function(init_value)
    return init_value or 0.0
end

local DoubleMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, Double)
        return init_value or 0.0
    end
}

setmetatable(Double, DoubleMt)