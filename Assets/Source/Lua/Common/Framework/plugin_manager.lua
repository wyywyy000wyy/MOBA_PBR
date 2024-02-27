local plugin_manager = class("plugin_manager", function(self)
    self._plugins = {}
    self._update_plugins = {}
end) 

P = P or {}

local function PCALL (plugin, func_name,...)
    local func = plugin[func_name]
    local ok, err = pcall(func, plugin, ...)
    if not ok then
        ELOG(string.format("call %s %s failed: %s", plugin.name, func_name, err))
    else
        LOGF("[Plugin] %s %s", plugin.name, func_name)
    end
end

local function MCALL(plugin, func_name,...)
    local func = plugin[func_name]
    local ok, err = pcall(func, plugin, ...)
    if not ok then
        ELOG(string.format("call %s %s failed: %s", plugin.name, func_name, err))
    end
end

function plugin_manager:load(config)
    for i, v in ipairs(config) do
        self:_load_plugin(v)
    end
end

function plugin_manager:load_plugin(name, is_service)
    local path 
    local plugin_root 
    local plugin_name
    if is_service then
        path = string.format("plugins/%s/service/", name)
        plugin_root = string.format("plugins/%s/", name)
        plugin_name = string.format("service_%s", name)
    else
        path = string.format("plugins/%s/", name)
        plugin_root = path
        plugin_name = string.format("plugin_%s", name)
    end
    local load = function()
        local plugin_path = string.format("%s%s", path, plugin_name)
        require(plugin_path)
        local plugin_class = T[plugin_name]
        if plugin_class.requires then
            for i, v in ipairs(plugin_class.requires) do
                local require_path = string.format("%s%s", plugin_root, v)
                require(require_path)
            end
        end

        local plugin = plugin_class(plugin_name)
        plugin.is_s = is_service
        self._plugins[plugin_name] = plugin
        if plugin_name == "task_manager" then
            self._tm = plugin
        end
        PCALL(plugin, "init")

        local define_path = string.format("%sdefine_%s", plugin_root, name)
        local define = require(define_path)
        if define then
            TM:_plugin_loaded(plugin, define)
        end
        if plugin.update then
            table.insert(self._update_plugins, plugin)
        end
    end
    local ok, err = pcall(load)
    if not ok then
        ELOG(string.format("load plugin failed: %s \n %s", plugin_name, err))
    end
    return self._plugins[plugin_name]
end

function plugin_manager:_load_plugin(config)
    local plugin = self._plugins[config.name]
    if plugin then
        return
    end
    if config.service then
        self:load_plugin(config.name, config.service)
        -- self:load_plugin(config.name)
    else
        self:load_plugin(config.name)
    end
end

function plugin_manager:find_plugin(name)
    return self._plugins[name]
end

function plugin_manager:update()
    for i, v in ipairs(self._update_plugins) do
        -- MCALL(v, "update")
        v:update()
    end
end

g_plugin_manager = g_plugin_manager or plugin_manager()
PM = g_plugin_manager