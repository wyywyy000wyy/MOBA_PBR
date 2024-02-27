local unpack = table.unpack
T.task = T.task or {}
task = T.task

task.tasks = task.tasks or {}
local tasks = task.tasks

local task = task



task.__index = function (t, k)
    local v = rawget(task, k)
    if v then
        return v
    end
    
    local task_class = tasks[k]
    if task_class then
        return function(pre_task_ins, ...)
            local task_list = pre_task_ins:get_task_list(true)
            local task_ins = task_class(...)
            task_list:add(task_ins)
            return task_ins
        end
    end
end

task.task_mt = {
    __call = function(_, task_name, task_data)
        local task_ins = {}
        setmetatable(task_ins, task)
        task_ins:constructor(task_name, task_data)
        return task_ins
    end
}
setmetatable(task, task.task_mt)

TASK_STATE = {
    WAIT = 0,
    EXCUTE = 1,
    FINISH = 2,
}

function task:constructor(name, task_data)
    self._n = name
    self._d = task_data
    self._s = TASK_STATE.WAIT
end

function task:get_task_list(create)
    local task_list = self._task_list
    if not task_list and create then
        task_list = T.task_list()
        self._task_list = task_list
        task_list:add(self)
    end
    return task_list
end

function task:excute()
    local task_list = self:get_task_list(true)
    TM:excute(task_list)
end

function task:on_excute()
    self._s = TASK_STATE.EXCUTE
    local on_excute = self._on_excute
    local op, result = on_excute(self, unpack(self._d))
    if op then
        self:finish(result)
    end
end

function task:finish(result)
    self._r = result
    self._s = TASK_STATE.FINISH
    TM:_task_finish(self)
end

local task_class_mt = {
    __call = function(...)
    end
}

function task_def(task_name, on_excute, properties, result_properties)
    properties = properties or {}
    local task_class = StructDef(task_name, properties)
    task_class._on_excute = on_excute
    local result_class = result_properties and StructDef(task_name .. "_r", result_properties)

    local task_wrapper = {
        name = task_name,
    }
    local mt = {
        __call = function(t, pre_task_ins, ...)
            if pre_task_ins == task then
                pre_task_ins = nil
            end
            -- local values = {...}
            local task_data = task_class(...)
            local task_ins = task(task_name, task_data)
            task_ins._on_excute = on_excute
            if pre_task_ins then
                local task_list = pre_task_ins:get_task_list(true)
                task_list:add(task_ins)
            end
            return task_ins
        end
    }
    setmetatable(task_wrapper, mt)
    tasks[task_name] = task_wrapper
    task[task_name] = task_wrapper
    return task_wrapper
end

function task:is_excute()
    return self._s == TASK_STATE.EXCUTE
end

function task:pr()
    local task_list = self:get_task_list()
    return task_list:pre_task()._r
end