

print("main run")

local action_list = {}

function add_require(fn,g_name,g_table)
    table.insert(action_list, {type = "require",file = fn,g_name = g_name,g_table = g_table or _G})
end

function do_load(mod)
    package.loaded[mod] = nil
    -- U.GUtils.CostTime("load " .. mod)
    local ret, msg = pcall(require, mod)
    if not ret then
        error(string.format("critial error! %s requires failed:%s", mod, msg))
    end
    return msg
end

require("Base/import")
require("Base/s_coroutine")



require("Base/global_func")

function run_all_action(action_complete, on_progress)
    local total = #action_list

    for idx, action in ipairs(action_list) do
        if(action.type == "require") then
            local rt = do_load(action.file)
            if(rt and action.g_name) then
                action.g_table[action.g_name] = rt
            end
        end

        if math.fmod(idx, 10) == 0 then
            if on_progress then
                on_progress(0.1+idx/total*0.7, "")
            end
            Yield()
        end
    end

    if on_progress then
        on_progress(0.8, "")
    end
    Yield()

    action_complete()
end

function reload()
    local total = #action_list
    for idx, action in ipairs(action_list) do
        if(action.type == "require") then
            local rt = do_load(action.file)
            if(rt and action.g_name) then
                action.g_table[action.g_name] = rt
            end
        end
    end

    INFO("===reload done===")
end

function main()
    collectgarbage("setpause", 100)
    collectgarbage("setstepmul", 500)

    math.randomseed(os.time())
    CS.XLoader.Initialize(true)

    coroutine.wrap(run_all_action)(function()

        -- CS.XLoader.LoadAsync("Effects/GameObject.prefab", typeof(U.GameObject),function(go)
        --     ELOG("loaded GameObject", go)
        --     U.GameObject.Instantiate(go)
        -- end)

        unload_unused_memory_on_next_frame()
        lock_g_variable()
    end)

end

main()