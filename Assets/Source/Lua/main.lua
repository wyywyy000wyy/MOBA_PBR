
require("base/import")
require("common/framework/class")
require("framework/file_system")
require("base/global_func")
require("common/framework/hot_require")
require_folder("framework/type")
require("base/s_coroutine")
require("base/behaviour_bridge")
require("base/game_state/game_state")
require("base/game_state/game_state_manager")
require("base/game_state/game_state_login", true)

require_folder("data", true)
-- require_folder("base/task")

require_folder("base/ui")

require_folder("framework")
PM:load_plugin("task_manager")

function main()
    collectgarbage("setpause", 100)
    collectgarbage("setstepmul", 500)
    math.randomseed(os.time())

    PM:load(require("plugins/plugin_manifest"))
    require("base/test")
    -- g_game_state_manager:push_state(T.game_state_login())
end

function update()
    PM:update()
end

main()