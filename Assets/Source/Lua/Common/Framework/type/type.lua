T = T or {}

-- local Type = class("Type", function(self, Name)
--     self.name = Name

--     local c = getmetatable(self)
--     c.__call = function(class_tbl, ...)
--         -- local value = {}
--         local value = {...}
--         setmetatable(value, self)

        
--         return value
--     end
-- end)

-- local Type = {}

-- local TypeMt = {
--     __call = function(class_tbl, ...)
--         -- local value = {}
--         local value = {...}
--         setmetatable(value, class_tbl)
--         return value
--     end
-- }

-- setmetatable(Type, TypeMt)


-- function TypeDef(Name)
--     T[Name] = Type(Name)
--     _G[Name] = T[Name]
-- end

-- TypeDef("String")
-- TypeDef("Int")
-- TypeDef("Long")
-- TypeDef("Float")
-- TypeDef("Bool")
-- TypeDef("Double")
lrequire("String")
lrequire("Int")
lrequire("Long")
lrequire("Float")
lrequire("Bool")
lrequire("Double")
lrequire("Key")

-- TypeDef("Key")

local Property = class("Property", function(self, Type, Name, DefaultValue, PropertyBit)
    self.type = Type
    self.name = Name
    self.default_value = DefaultValue
    self.property_bit = PropertyBit
end)

------------------- PropertyDefine -------------------
function PD(Type, Name, DefaultValue, PropertyBit)
    return Property(Type, Name, DefaultValue, PropertyBit)
end

lrequire("Struct")

pre_type = pre_type or type
local pre_type = pre_type
function get_type(t)
    local tn = pre_type(t)
    if tn == "table" then
        return getmetatable(t)
    end
    return tn
end
