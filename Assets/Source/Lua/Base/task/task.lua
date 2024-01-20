local task = class("task",function(self)
end)

function task:execute()
    self:on_execute()
end

function task:on_execute()
end

function task:add_fcb(fcb)
    self._fcbs = self._fcbs or {}
    table.insert(self._fcbs, fcb)
end

function task:finish()
    self:on_finish()
end

function task:on_finish()
    if self._fcbs then
        for i, v in ipairs(self._fcbs) do
            SAFE_CALL(v)
        end
    end
end