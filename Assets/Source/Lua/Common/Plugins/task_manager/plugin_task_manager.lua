local plugin_task_manager = plugin_def("plugin_task_manager")

plugin_task_manager.requires = {
    "event/event",
    "event/task",
    "event/action",
    "event/task_list",
    "event/task_define",
}

function plugin_task_manager:init()
    TM = TM or self
    self.tasks = T.task.tasks
    self.actions = {}
    self.pending_tasks = {}
end

function plugin_task_manager:_plugin_loaded(plugin, plugin_define)
    local action = plugin_define.action
    if action then
        for i, v in ipairs(action) do
            self:_plugin_action(plugin, v)
        end
    end
end

function plugin_task_manager:_plugin_action(plugin, action_class)
    if plugin[action_class.name] then
        action_class.plugin = plugin
        self.tasks[action_class.name] = action_class
        self.actions[action_class.name] = plugin
    else

    end
end

function plugin_task_manager:update()
    -- LOG("plugin_task_manager:update()")
    local pending_tasks = self.pending_tasks
    self.pending_tasks = {}
    for i, v in ipairs(pending_tasks) do
        self:_excute_task(v)
    end
end

function plugin_task_manager:excute(task_list)
    local cur_task = task_list:get_cur_task()
    if not cur_task then
        task_list:finish()
        return
    end
    table.insert(self.pending_tasks, cur_task)
    -- self:_excute_task(cur_task)
end


function plugin_task_manager:_excute_task(task)
    if task:is_excute() then
        return
    end

    task:on_excute()
end

function plugin_task_manager:_task_finish(task)
    local task_list = task:get_task_list()
    task_list:next()
    self:excute(task_list)
end