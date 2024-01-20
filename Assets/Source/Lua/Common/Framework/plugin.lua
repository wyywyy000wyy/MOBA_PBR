local plugin = class("plugin", function(self, plugin_mamager)
    self._manager = plugin_mamager
end)

function plugin_def(name, _class)
    local plugin_class = _class or class(name, plugin, function(self, plugin_manager)
        plugin._ctor(self, plugin_manager)
    end)
    plugin_class.name = name
    return plugin_class
end

function plugin:init()
end

function plugin:enable()
end

function plugin:disable()
end

function plugin:destroy()
end