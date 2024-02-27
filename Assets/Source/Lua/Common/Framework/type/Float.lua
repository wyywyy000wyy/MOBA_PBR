T.Float = T.Float or {}
Float = T.Float
local Float = Float
Float.name = "Float"
Float.v = 0.0
Float.__tostring = function(self)
    return self.v
end
Float.ctor = function(init_value)
    return init_value or 0.0
end

local FloatMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, Float)
        return init_value or 0.0
    end
}

setmetatable(Float, FloatMt)