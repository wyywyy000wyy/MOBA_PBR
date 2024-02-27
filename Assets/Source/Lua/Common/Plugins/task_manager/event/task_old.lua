local task = T.task
local setmetatable = setmetatable
local tasks

local function error_hander(h)
    ELOG(string.format("lua:%s \r\n %s", h, debug.traceback()))
end

if not task then
    task = {
        tasks = {},
        task_instances = {}
    }
    local task_instances = task.task_instances
    task.__index = task
    local _mt = {
        __index = function(task_ins, task_name)
            local a = 1
            local func = task[task_name]
            if func then
                return func
            end
            return task.next_task(task_ins, task_name)
        end,
    }
    local mt_task = {
        __index = function(task_ins, task_name)
            local func = rawget(task, task_name)
            if func then
                return func
            end
            return task.next_task(task_ins, task_name)
        end,
        __call =  function(_, task_name)
            local task_ins = setmetatable({}, _mt)
            if task_name then
                task_instances[task_name] = task_ins
            end
            return task_ins
        end
    }
    setmetatable(task, mt_task)
    T.task = task
    Task = task
end
tasks = task.tasks


function task.task_safe_call(task_ins, func)
    if not func then
        return true
    end
    local params = task_ins._params or {}
    local sus, ret = xpcall(func, error_hander, task_ins, unpack(params))
    if not sus then
        local task_list = task_ins._task_list
        if task_list then
            task_list:cancel()
        end
    end
    return ret
end

function task.wrapper(on_excute, on_finish)
    return function(...)
        local task = task()
        task._params = {...}
        task.on_excute = on_excute
        task.on_finish = on_finish
        return task
    end
end

local task_safe_call = task.task_safe_call

function task.next_task(_, task_name)
    local task_class = tasks[task_name]
    if not task_class then
        return nil
    end
    return function(task_ins,...)
        local task_name_11 = task_name
        -- task_ins = rpg_task_instance2 or task_ins
        local task = task_class(...)
        task._task_name = task_name
        task_ins:task_list():add(task)
        return task
    end
end

function task.task_list(task_ins)
    local task_list = task_ins._task_list
    if not task_list then
        task_list = T.task_list("task_list")
        task_ins._task_list = task_list
        task_list:add(task_ins)
    end
    return task_list
end


function task.excute(task_ins)
    local task_list = task_ins:task_list()
    task_list:excute()
    return task_ins
end

function task.set_result(task_ins, ...)
    task_ins._task_list:set_result(...)
end

function task.get_result(task_ins, result_name)
    local task_list = task_ins._task_list
    local result = task_list:get_result(result_name)
    if result then
        return unpack(result)
    end
end

function task.cancel(task_ins)
    local task_list = task_ins._task_list
    task_list:cancel()
end

function task.restart(task_ins)
    local task_list = task_ins._task_list
    task_list:restart()
end

function task.finish(task_ins, result_1, ...)
    if task_ins._finish then
        return
    end
    if result_1 then
        task_ins._result = {result_1, ...}
    end
    if task_ins.on_finish then
        task_safe_call(task_ins, task_ins.on_finish)
    end
    task_ins._finish = true
    if task_ins._task_list then
        task_ins._task_list:do_next()
    end
end

