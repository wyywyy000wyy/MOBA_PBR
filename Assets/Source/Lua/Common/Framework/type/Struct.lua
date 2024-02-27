local unpack = table.unpack
T.Struct = T.Struct or {}
Struct = T.Struct
local Struct = Struct
Struct.name = "Struct"
Struct.v = 0

local StructMt = {
    __call = function(_, init_value)
        if not init_value then
            return nil
        end
        local struct_type  = getmetatable(init_value)
        return struct_type(unpack(init_value))
    end
}

Struct.__call = function(struct_type, ...)
    local dvlues = {...}
    for i, property in ipairs(struct_type.properties) do
        -- value[property.name] = property.type(dvlues[i] or property.default_value)
        dvlues[i] = property.type(dvlues[i] or property.default_value)
    end
    setmetatable(dvlues, struct_type)
    return dvlues
end 
Struct.ctor = function(init_value)
    if not init_value then
        return nil
    end
    local struct_type = getmetatable(init_value)
    return struct_type(unpack(init_value))
end

StructDef = function(Name, properties)
    local k2i = {
    }
    local struct_type 
    struct_type = {
        name = Name,
        properties = properties,
        __tostring = function(self)
            local t = {
                "{"
            }
            for i, property in ipairs(properties) do
                t[#t + 1] = property.name .. " = " .. self[property.name]
            end
            t[#t + 1] = "}"
            return table.concat(t, "\n")
        end,
        __index = function(self, k)
            local idx = k2i[k]
            if idx then
                return rawget(self, idx)
            end
        end,
        __call = Struct.__call
    }
    for i, property in ipairs(properties) do
        k2i[property.name] = i
    end

    setmetatable(struct_type, Struct)
    return struct_type
end

setmetatable(Struct, StructMt)