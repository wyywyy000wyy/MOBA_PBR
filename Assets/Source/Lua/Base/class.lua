
local none = {
    type="none"
}
-- local string = {
--     type="string"
-- }
local int = {
    type="number"
}

local gclass = {
    
}

gclass[none] = none
gclass[string] = {type="string"}
gclass[int] = int

local copy_field = function(t)
    local rt = { __data = {}}
    for k, v in pairs(t.__data or {}) do
        rt.__data[k] = v
    end
    return rt
end

local function findenv(f)
    local level = 1
    repeat
        local name, value = debug.getupvalue(f,level)
        if name == "_ENV" then
            return level, value
        end
        level = level + 1
    until name == nil
    return nil
end


local getfenv = function(f)
    return (select(2, findenv(f)) or _G)
end

local setfenv = function(f, t)
    local level = findenv(f)
    if level then
        debug.setupvalue(f, level, t)
    end
    return f
end

function class(...)
    local cls_data = ...
    local cls = {}

    cls.__cls_data = {}

    local super_cls = cls_data["__super"]
    if super_cls and super_cls.__cls_data then
        for k, v in pairs(super_cls.__cls_data) do
            cls.__cls_data[k] = v
        end
    end

    for k, v in pairs(cls_data) do
        cls.__cls_data[k] = gclass[v] and gclass[v].type or v
        -- mt.__data[k] = v

    end

    setmetatable(cls, {
        __call = function()
            local new_t = {
                __data = {}
            }

            setmetatable(new_t, cls)

            return new_t
        end
    })

    cls.__index = function(t, key)
        local r = t.__data[key]
        if r then
            return r
        end
        r = cls.__cls_data[key]
        if r and type(r) == "function" then
            local newG = {}
            setmetatable(newG, {__index = _G})
            for k, v in pairs(t.__data) do
                newG[k]=v
            end
            setfenv(r, newG)
        end
        return r 
    end

    cls.__newindex = function(t, key, value)
        if cls.__cls_data[key] and cls.__cls_data[key] ~= type(value) then
            print(string.format("类型不匹配: %s 的类型是 %s", key, cls.__cls_data[key]))
            return
        end
        t.__data[key] = value
    end

    return cls
end

-- A =
--     class {
--     name = string,
--     age = int,
--     foo = function()
--         print('from A', name, age)
--     end,
-- }

-- B =
--     class {
--     __super = A,
--     foo = function()
--         print('from B', name, age)
--     end
-- }
-- -- print(DT(A))

-- local a = A()
-- -- print(DT(A))
-- a.name = 'hanmeimei'
-- a.age = 17
-- a.foo()
-- -- print(DT(a))

-- local b = B()
-- b:foo()
-- b.name = 'lilei'
-- b.age = 18
-- b:foo()

-- -- a.name = 20
-- -- a.age = '20'
-- b.foo = 'x'
