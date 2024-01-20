local plugin_service = class("plugin_service", T.plugin, function(self, plugin_manager)
    T.plugin._ctor(self, plugin_manager)
end)

function plugin_service_def(name)
    local service_class = class(name, plugin_service, function(self, plugin_manager)
        plugin_service._ctor(self, plugin_manager)
    end)
    plugin_def(name, service_class)
    service_class.action = {}
    return service_class
end