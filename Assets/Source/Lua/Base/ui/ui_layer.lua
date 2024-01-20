local ui_layer = class("ui_layer",function(self, layer_id) 
    self._windows = {}
    self._layer_id = layer_id
    self._ui_layer = E.GameObject.Instantiate(g_ui_manager._ui_layer, g_ui_manager._ui_layer.transform.parent)
    self._ui_layer.gameObject:SetActive(true)
    self._ui_layer.name = "ui_layer_" .. layer_id
end)

function ui_layer:add_window(window)
    for i, v in ipairs(self._windows) do
        if v == window then
            return
        end
    end
    table.insert(self._windows, window)
    window._ref = window._ref + 1
    if not window:is_loaded() then
        window:load()
        window._load_task:add_fcb(function() 
            self:check_visible()
        end)
    end
end

function ui_layer:remove_window(window)
    for i, v in ipairs(self._windows) do
        if v == window then
            table.remove(self._windows, i)
            window._ref = window._ref - 1
            if self:is_top_layer() then
                window:set_visible(false)
            end
            break
        end
    end
end

function ui_layer:hide_all()
    local cur_top_layer = g_ui_manager:get_top_layer()
    local next_top_layer = nil
    if cur_top_layer == self then
        g_ui_manager._top_layer = g_ui_manager._top_layer - 1
        next_top_layer = g_ui_manager._layers[g_ui_manager._top_layer]
    else
        next_top_layer = cur_top_layer
    end

    for i, v in ipairs(self._windows) do
        v._ref = v._ref - 1
        if not next_top_layer:has_window(v) then
            v:set_visible(false)
        end
    end
    self._windows = nil

    for i, v in ipairs(next_top_layer._windows) do
        v:set_visible(true, next_top_layer)
    end
    if cur_top_layer == self then
    end
    g_ui_manager._cur_show_layer = next_top_layer
end

function ui_layer:check_visible()
    if not self:is_top_layer() then
        return false
    end
    if not self:check_all_windows_loaded() then
        return false
    end

    local cur_show_layer = g_ui_manager:cur_show_layer()
    if cur_show_layer ~= self then
        if cur_show_layer then
            for i, v in ipairs(cur_show_layer._windows) do
                if not self:has_window(v) then
                    v:set_visible(false)
                end
            end
        end
        g_ui_manager._cur_show_layer = self
        for i, v in ipairs(self._windows) do
            v:set_visible(true, self)
        end
    end
end

function ui_layer:check_all_windows_loaded()
    for i, v in ipairs(self._windows) do
        if not v:is_loaded() then
            return false
        end
    end
    return true
end

function ui_layer:has_window(window)
    for i, v in ipairs(self._windows) do
        if v == window then
            return true
        end
    end
    return false
end

function ui_layer:is_top_layer()
    return self == g_ui_manager:get_top_layer()
end