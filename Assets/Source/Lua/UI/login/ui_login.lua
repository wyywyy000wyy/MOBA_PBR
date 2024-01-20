local ui_login = class("ui_login",T.ui_window, function(self)
end)


function ui_login:on_active()
    LOG("ui_login:on_active")
end

function ui_login:on_show()
    LOG("ui_login:on_show")
    self._ui._user_name.text = "user_666"
    self._ui._pass_word.text = "pass_123456"

    self:on_click(self._ui._btn_login, function()
        local user_name = self._ui._user_name.text
        local pass_word = self._ui._pass_word.text
        LOG("user_name:" .. user_name .. " pass_word:" .. pass_word)
    end)
end

function ui_login:on_hide()
    LOG("ui_login:on_hide") 
end
  
function ui_login:on_deactive()
    LOG("ui_login:on_deactive") 
end  