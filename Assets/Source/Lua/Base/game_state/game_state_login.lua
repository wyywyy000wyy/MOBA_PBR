local game_state_login = class("game_state_login",T.game_state, function(self)
    T.game_state._ctor(self)
end)

function game_state_login:on_enter()
    LOG("game_state_login:on_enter")
    g_ui_manager:push_window("ui_login")
end