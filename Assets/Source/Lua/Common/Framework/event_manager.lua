local event_manager = class("event_manager", function(self)
    self._events = {}
end)

function event_manager:post_event(event)
    local t = self._events[event]
    if t then
        for i, v in ipairs(t) do
            v(event)
        end
    end
end

function event_manager:add_event_handle(event, handle)
    self._events[event] = self._events[event] or {}
    table.insert(self._events[event], handle)
end

function event_manager:remove_event_handle(event, handle)
    if self._events[event] then
        for i, v in ipairs(self._events[event]) do
            if v == handle then
                table.remove(self._events[event], i)
                break
            end
        end
    end
end