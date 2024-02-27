T = T or {}

function class(pre, base, _ctor)

    local c
    if type(pre) =="string" then
        c= T[pre] or {}
        T[pre] = c
    else
        c= pre or {} -- a new class instance
    end
    if not _ctor and type(base) == "function" then
        _ctor = base
        base = nil
    elseif type(base) == "table" then
        -- our new class is a shallow copy of the base class!
        for i, v in pairs(base) do
            if type(v) ~= "function" then
                c[i] = v
            end
        end
        c._base = base
    end

    if type(pre) =="string" then
        c._cls_name = pre
        pre = T[pre]
    end
    -- the class will be the metatable for all its objects,
    -- and they will look up their methods in it.
    c.__index = c

    -- expose a constructor which can be called by <classname>(<args>)
    local mt = pre and getmetatable(pre) or {}

    if type(base) == "table" then
        mt.__index = base
    end
    mt.__call = function(class_tbl, ...)
        local obj = {}
        setmetatable(obj, c)

        if _ctor then
            _ctor(obj, ...)
        end
        return obj
    end

    c._ctor = _ctor
    c.is_a = function(self, klass)
        local m = getmetatable(self)
        while m do
            if m == klass then
                return true
            end
            m = m._base
        end
        return false
    end
    setmetatable(c, mt)
    return c
end
