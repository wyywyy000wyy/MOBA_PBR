
------------------------------------------------------------------------------------------


------------------------------------------------------------------------------------------
-- string
local function _stringify_lvalue(t, deep)
    if type(t) == 'table' then
        if deep <= 0 then
            return '[\'(table)\']'
        else
            local s = {}
            for k, v in pairs(t) do
                table.insert(s, string.format('%s=%s', _stringify_lvalue(k, deep - 1), _stringify_rvalue(v, deep - 1)))
            end
            return string.format('[{%s}]', table.concat(s, ','))
        end
    elseif type(t) == 'string' then
        if tonumber(t) then
            return string.format('[\'%s\']', t)
        else
            return t
        end
    elseif type(t) == 'userdata' or type(t) == 'function' then
        return string.format('\'(%s)\'', t)
    else
        return string.format('[%s]', t)
    end
end

local function _stringify_rvalue(t, deep)
    if type(t) == 'table' then
        if deep <= 0 then
            return '\'..\''
        else
            local s = {}
            for k, v in pairs(t) do
                table.insert(s, string.format('%s=%s', _stringify_lvalue(k, deep - 1), _stringify_rvalue(v, deep - 1)))
            end
            return string.format('{%s}', table.concat(s, ','))
        end
    elseif type(t) == 'string' then
        return string.format('\'%s\'', t)
    elseif type(t) == 'userdata' or type(t) == 'function' then
        return string.format('\'(%s)\'', type(t))
    else
        return tostring(t)
    end
end

function stringify(t)
    return _stringify_rvalue(t, 5)
end

local function error_hander(h)
    ELOG(h)
end
function SAFE_CALL(f, ...)
    return xpcall(f, error_hander, ...)
end


------------------------------------------------------------------------------------------


------------------------------------------------------------------------------------------
-- dump table

local function _dump_value(v, depth,map)
    depth = depth or 1
    map = map or {}

    if type(v) == 'string' then
        return string.format('%q', v)
    elseif type(v) == 'number' then
        return tostring(v)
    elseif type(v) == 'boolean' then
        return (v and 'true') or 'false'
    elseif type(v) == 'table' then
        if(map[v]) then
            return map[v];
        end
        map[v] = tostring(v);
        if(depth > 100) then
            return "too many depth; ignore"
        end
        local rt = "";
        for k, v in pairs(v) do
            rt = string.format("%s%s[%s] = %s;\n",rt,string.rep('\t', depth),_dump_value(k, depth + 1,map),_dump_value(v, depth + 1,map))
        end
        return string.format("{\n%s%s}",rt,string.rep('\t', depth - 1))
    elseif type(v) == 'userdata' then
        return "userdata"
    elseif type(v) == 'function' then
        return "function"
    else
        return "unknown type"
    end
end

local function _beautify_log(str, color)
    if _PUBLISH then
        return str
    end

    color = color or "white"
    local output = ""
    for log in string.gmatch(str, "([^\n]+)") do
        output = string.format("%s<color=%s>%s</color>\n", output, color, log)
    end
    return output
end

local function contact_parm(...)
    local n = select('#',...)
    local tb = table.pack(...)
    local s = ""
    for i = 1,n do
        s = s .. tostring(tb[i]) .."\t"
    end
    return s
end

function DT(t, force_in_release, step)

    return _dump_value(t,step)
end

------------------------------------------------------------------------------------------
-- logs
function LOG(...)
    if not _PUBLISH then
        E.Debug.Log(  contact_parm(...).."\r\n".. debug.traceback("",2))
    end
end

function LOGF(format, ...)
    if not _PUBLISH then
        E.Debug.Log(  string.format(format, ...).."\r\n".. debug.traceback("",2))
    end
end

function ELOG(...)
    E.Debug.LogError(  contact_parm(...) ..  debug.traceback())
end

function DLOG(...)
    if not _PUBLISH then
        E.Debug.Log(  contact_parm(...).."\r\n".. debug.traceback("",2))
    end
end

function INFO(...)
    if not _PUBLISH then
        if _UNITY_EDITOR then
            E.Debug.Log(_beautify_log(  contact_parm(...), "#ffff00"))
        else
            E.Debug.Log(  contact_parm(...))
        end
    end
end

function dbg(...)
    LOG(...)
end

function dbgt(...)
    E.Debug.Log(_beautify_log(DT({...},false),"#00ff00"))
end
----------------------------------------------------------------------------------------

g_variable = {}

function lock_g_variable()
	local __g = _G
	setmetatable(g_variable, {
	    __newindex = function(_, name, value)
	        rawset(__g, name, value)
	    end,

	    __index = function(_, name)
	        return rawget(__g, name)
	    end
	})

	-- disable create unexpected global variable
	setmetatable(__g, {
	    __newindex = function(_, name, value)
	        error(string.format("USE \" g_variable.%s = value \" INSTEAD OF SET GLOBAL VARIABLE", name), 0)
	    end
	})
end

-----------------------------------------------------------------------------------------
--MUST KNOW: this function will SLOW DOWN the game!!
is_unload_unused_memory_processing_ = false
function unload_unused_memory_on_next_frame()
    if is_unload_unused_memory_processing_ then
        return
    end

    coroutine.wrap(function()
        is_unload_unused_memory_processing_ = true

        INFO("===== unload unused memory begin =====")
        -- local count = 1
        while not collectgarbage("step", 256) do
            -- INFO("== collect garbage cycle %d", count)
            -- count = count + 1
            Yield()
        end
        collectgarbage("restart")
        Yield(E.Resources.UnloadUnusedAssets())
        INFO("===== unload unused memory end =====")

        is_unload_unused_memory_processing_ = false
    end)()
end

function extend_tbl(t,base)
    for k,v in pairs(base) do
        if not t[k] then
            t[k] = v
        end
    end
end