local task_c = class("task_c",function(self)
end)

function task_c:execute()
    self:on_execute()
end

function task_c:on_execute()
end

function task_c:add_fcb(fcb)
    self._fcbs = self._fcbs or {}
    table.insert(self._fcbs, fcb)
end

function task_c:finish()
    self:on_finish()
end

function task_c:on_finish()
    if self._fcbs then
        for i, v in ipairs(self._fcbs) do
            SAFE_CALL(v)
        end
    end
end