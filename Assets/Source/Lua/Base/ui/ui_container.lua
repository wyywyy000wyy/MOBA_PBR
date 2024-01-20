ui_container = ui_container or {}
local ui_container = ui_container

local ui_container_mt = {
    __call = function(self, ui_root)
        local ui_map = {}
        for i = 0, ui_root.uiList.Count - 1 do
            local ui = ui_root.uiList[i]
            ui_map[ui.name] = ui.comp
        end

        local t = {}
        local mt = {
            __index = function(t, k)
                return ui_map[k]
            end
        }
        setmetatable(t, mt)
        return t
    end
}

setmetatable(ui_container, ui_container_mt)