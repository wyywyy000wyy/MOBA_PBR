local task_safe_call = T.task.task_safe_call
local task_list = class("task_list", function(self, task_name, ...)
    self._task_name = task_name
    self._task_queue = {...}
    -- self._finish_queue = {}
    self._cur_task_idx = 1
end)

function task_list:get_cur_task()
    return self._task_queue[self._cur_task_idx]
end

function task_list:next()
    self._cur_task_idx = self._cur_task_idx + 1
end

function task_list:add(task)
    table.insert(self._task_queue, task)
    task._task_list = self
end

function task_list:pre_task()
    return self._task_queue[self._cur_task_idx - 1]
end

function task_list:finish()
end