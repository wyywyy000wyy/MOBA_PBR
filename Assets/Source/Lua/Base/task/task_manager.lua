local task_manager = class("task_manager",function(self)
    self._tasks = {}
end)

function task_manager:execute(task)
    if self._tasks[task] then
        return
    end
    self._tasks[task] = task
    task:execute()
end

g_task_manager = g_task_manager or task_manager()