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

function MapController:OnBeginDrag()
    ELOG("OnBeginDrag", U.Input.mousePosition)
    self.start_point = U.Input.mousePosition
    self.camera_pos = self.camera.transform.position
end

function MapController:OnEndDrag()
    ELOG("OnEndDrag")
    
end

function MapController:OnDrag()
    -- ELOG("OnDrag", U.Input.touchCount)
    local dt = U.Input.mousePosition - self.start_point
    ELOG("OnDrag", dt.x, dt.y)

    self.camera.transform.position = self.camera_pos + U.Vector3(dt.x,0,dt.y) * 0.1
end