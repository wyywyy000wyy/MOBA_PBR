local ui_manager = class("ui_manager", function(self) 
    self._layers = {}
    self._top_layer = 1
    self._cur_show_layer = nil
    self._windows = {}
    self._ui_root = E.GameObject.Find("ui_root")
    self._ui_layer = self._ui_root.transform:Find("ui_layer")
    self._ui_layer.gameObject:SetActive(false)
end)

function ui_manager:open_window(window_name, ...)
    local layer = self._layers[self._top_layer]
    if not layer then
        layer = T.ui_layer(self._top_layer)
        self._layers[self._top_layer] = layer
    end
    layer:add_window(self:get_window(window_name))
end

function ui_manager:close_window(window_name)
    local layer = self._layers[self._top_layer]
    if not layer then
        return
    end
    local window = self:get_window(window_name)
    layer:remove_window(window)
end

function ui_manager:pop_layer()
    local layer = self._layers[self._top_layer]
    if not layer then
        return
    end
    layer:hide_all()
end

function ui_manager:push_window(window_name, ...)
    self._top_layer = self._top_layer + 1
    local layer = self._layers[self._top_layer]
    if not layer then
        layer = T.ui_layer(self._top_layer)
        self._layers[self._top_layer] = layer
    end
    layer:add_window(self:get_window(window_name))
end

function ui_manager:get_window(window_name)
    local window = self._windows[window_name]
    if not window then
        local config = ui_config[window_name]
        require("ui/" ..config.code)

        window = T[window_name]()
        T.ui_window._ctor(window, window_name)
        self._windows[window_name] = window
    end
    return window
end

function ui_manager:get_top_layer()
    return self._layers[self._top_layer]
end

function ui_manager:cur_show_layer()
    return self._cur_show_layer
end

g_ui_manager = g_ui_manager or ui_manager()