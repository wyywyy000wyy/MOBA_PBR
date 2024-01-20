
require("Base/import")
require("Common/Framework/class")
require("Framework/file_system")
require("Base/global_func")
require("Common/Framework/hot_require")
require("Base/s_coroutine")
require("Base/behaviour_bridge")
require("Base/game_state/game_state")
require("Base/game_state/game_state_manager")
require("Base/game_state/game_state_login")

require_folder("Data", true)
require_folder("Base/task")

require_folder("Base/ui")

require_folder("Framework")

function main()
    collectgarbage("setpause", 100)
    collectgarbage("setstepmul", 500)
    math.randomseed(os.time())
 
    g_game_state_manager:push_state(T.game_state_login())
end

main()