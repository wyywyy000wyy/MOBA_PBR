
local property_enum ={
    save = 1,

    client = 30,
    server = 31
}

local property = class("property", function(self, default_value, property_bits)
    self._v = default_value
    self._pb = property_bits
end)


function property_bits(...)
    local bit = 0
    local args = {...}
    for i, v in ipairs(args) do
        bit = bit | (1 << v)
    end
    return bit
end

function default_property(value)
    return property(value, property_bits(property_enum.save, property_enum.client, property_enum.server))
end

function id_property()
end

function str_property()
end



function passwd_property()
end