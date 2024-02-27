T.String = T.String or {}
String = T.String
local String = String
String.name = "String"
String.v = ""
String.__tostring = function(self)
    return string.format("\"%s\"", self.v)
end
String.ctor = function(init_value)
    return init_value or ""
end

local StringMt = {
    __call = function(_, init_value)
        -- local value = {
        --     v = init_value
        -- }
        -- setmetatable(value, String)
        return init_value or ""
    end
}

setmetatable(String, StringMt)