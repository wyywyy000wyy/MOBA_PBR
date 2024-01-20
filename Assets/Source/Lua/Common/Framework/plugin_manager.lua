local plugin_manager = class("plugin_manager", function(self)
    self._plugins = {}
end) 

function plugin_manager:load(config)
    for i, v in ipairs(config) do
        self:_load_plugin(v)
    end
end

function plugin_manager:_load_plugin(config)
    local plugin = self._plugins[config.name]
    if plugin then
        return
    end
    require(config.path)
    local plugin_class = T.plugin[config.name]
    plugin = plugin_class(config.name)
    self._plugins[config.name] = plugin
    plugin:load(config)
end