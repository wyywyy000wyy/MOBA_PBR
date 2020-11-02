MapController = class(BehaviourBridge)

function MapController:ctor(SceneScriptRoot)
    self.scene_script_root = U.GameObject.Find("Plane")
    self.camera = U.GameObject.Find("Main Camera")
    self:bind(self.scene_script_root)
end

function MapController:bridge_type()
    return U.LuaBridgeType.Dragable3D
end


function MapController:bind_behaviour_funcs()
    local Func = U.UnityBehaviourFunc

    return {
        [Func.OnBeginDrag] = self:wrap_func("OnBeginDrag"),
        [Func.OnEndDrag] =  self:wrap_func("OnEndDrag"),
        [Func.OnDrag] =  self:wrap_func("OnDrag"),
    }
end

function MapController:OnBeginDrag(eventData)
    ELOG("OnBeginDrag")
end

function MapController:OnEndDrag(eventData)
    ELOG("OnEndDrag")
end

function MapController:OnDrag(mousePosition)
    ELOG("OnDrag")
end