-- local action = class("action",T.task)

function action_def(name, ...)
    local params = {}
    local _params = {...}
    for i, t in ipairs(_params) do
        params[i] = PD(t, "p" .. i)
    end

    local action_class = task_def(name, function(action, ...)
        local plugin = TM.actions[name]
        return plugin[name](plugin, action, ...)
    end, params)
    return action_class
end

AD = action_def
