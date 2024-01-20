local ui_window = class("ui_window", function(self, window_name) 
    self._window_name = window_name

    self._ui_root = nil
    self._load_task = nil
    self._ref = 0
    self._active = false
end)

function ui_window:load()
    if self._load_task then
        return
    end
    self._load_task = T.task_load_asset(self._window_name)

    self._load_task:add_fcb(function()
        self._ui_root = self._load_task._asset:GetComponent(typeof(E.UIContainer))
        self._ui = ui_container(self._ui_root)
        self._load_task = nil
    end)
    g_task_manager:execute(self._load_task)
end

function ui_window:destroy()
    if self._ui_root then
        self._ui_root:destroy()
        self._ui_root = nil
    end
end


function ui_window:is_loaded()
    return self._ui_root ~= nil
end

function ui_window:set_visible(visible, ui_layer)
    self._visible = visible
    if self._ui_root then
        self._ui_root.gameObject:SetActive(visible)
        if visible then
            self._ui_root.transform:SetParent(ui_layer._ui_layer, false)
            if not self._active then
                self._active = true
                self:on_active()
            end
            self:on_show()
        else
            self:on_hide()
            if self._ref == 0 then
                self._active = false
                self:on_deactive()
            end
        end
    end
end

function ui_window:on_active()
end

function ui_window:on_show()
end

function ui_window:on_hide()
end

function ui_window:on_deactive()
end


function ui_window:on_click(btn, func)
    btn.onClick:RemoveAllListeners()
    btn.onClick:AddListener(func)
end