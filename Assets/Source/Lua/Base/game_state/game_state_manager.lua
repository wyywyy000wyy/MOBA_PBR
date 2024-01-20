local game_state_manager = class("game_state_manager",function(self)
end)

function game_state_manager:push_state(state)
    if self.current_state then
        self.current_state:on_exit()
    end
    self.current_state = state
    self.current_state:on_enter()
end

g_game_state_manager = g_game_state_manager or game_state_manager()