-- local action = class("action",T.task)

function action_def(name, params)
    local action_class = task_def(name, function(action, ...)
        local plugin = TM.actions[name]
        return plugin[name](plugin, action, ...)
    end, params)
    return action_class
end
