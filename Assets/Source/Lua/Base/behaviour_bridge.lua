---@class BehaviourBridge
BehaviourBridge = class()

function BehaviourBridge:ctor(go)
    self:bind(go)
end

function BehaviourBridge:bridge_type()
    return U.LuaBridgeType.base
end

function BehaviourBridge:bind(go)
    if go then
        self.gameObject = go
        self.transform = go.transform
        local bt = self:bridge_type()
        local comp = U.LuaBridgeBase.Bind(bt, go, self, self:bind_behaviour_funcs())
        ELOG(DT(self:bind_behaviour_funcs()))
        if comp then
            self.lua_bridge__ = comp
            if self.awake then
                self:awake()
            end
            self.alive__ = true
        end
    end
end

function BehaviourBridge:lua_bridge()
    return self.lua_bridge__
end

function BehaviourBridge:on_destroy()
    self.alive__ = false
    BehaviourBridge.free_userdate_ref(self,0)
end

function BehaviourBridge.free_userdate_ref(tb,depth)
    if depth > 2 then return end

    for k, v in pairs(tb) do
        if type(v) == "table" and not getmetatable(v) then
            BehaviourBridge.free_userdate_ref(v, depth+1)
        elseif type(v) == "userdata" then
            tb[k] = nil
        end
    end
end

function BehaviourBridge:alive()
    return self.alive__
end

function BehaviourBridge:wrap_func(func_name)
    return function(t, ...)
        local func = t[func_name]
        if func then
            func(t, ...)
        end
    end
end

function BehaviourBridge:bind_behaviour_funcs()
    local Func = U.UnityBehaviourFunc

    return {
        [Func.Start] = self:wrap_func("start"),
        [Func.OnDestroy] =  self:wrap_func("on_destroy"),
    }
end

---@param name System.String
---@return UnityEngine.Transform
function BehaviourBridge:get_child(name)
    return self.transform:Find(name)
end

---@param name System.String
---@return UnityEngine.GameObject
function BehaviourBridge:get_child_object(name)
    local trans = self.transform:Find(name)
    if trans then
        return trans.gameObject
    end
end

---@return UnityEngine.Transform
function BehaviourBridge:get_trans()
    return self.transform
end

---@return UnityEngine.Vector3
function BehaviourBridge:get_pos()
    return self.transform.position
end

---@param pos UnityEngine.Vector3
function BehaviourBridge:set_pos(pos)
    self.transform.position = pos
end