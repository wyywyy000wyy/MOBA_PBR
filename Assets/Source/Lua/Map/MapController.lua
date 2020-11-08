MapController = class(BehaviourBridge)

function MapController:ctor(SceneScriptRoot)
    self.scene_script_root = U.GameObject.Find("SceneScriptRoot")
    self.camera = U.GameObject.Find("Main Camera")
    self.camera_forward = U.GameObject.Find("Main Camera").transform.forward
    self:bind(self.scene_script_root)

    self.isDraging = false;
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
        [Func.Update] =  self:wrap_func("Update"),
    }

end

function MapController:Update()
    if (U.Input.GetMouseButtonDown(0)) then
        if(U.EventSystem.current:IsPointerOverGameObject() == false) then
            self:OnBeginDrag();
        end
    elseif(U.Input.GetMouseButtonUp(0) and self.isDraging) then
        self:OnEndDrag();      
    elseif(U.Input.GetMouseButton(0) and self.isDraging) then   
        self:OnDrag();
    end
end

function MapController:OnBeginDrag()
    -- LOG("OnBeginDrag", U.Input.mousePosition)
    self.isDraging = true;

    self.start_point = U.Input.mousePosition
    self.camera_pos = self.camera.transform.position

    self.mode = 1
    if U.Input.touchCount == 2 then
        self.touch_dis = (U.Input.touches[0].position - U.Input.touches[1].position).magnitude
        self.mode = 2
    end
end

function MapController:OnEndDrag()
    -- LOG("OnEndDrag")
    self.isDraging = false
    self.touch_dis = nil
    self.mode = nil
end

function MapController:OnDrag()
    -- ELOG("OnDrag", U.Input.touchCount)
    local dt = U.Input.mousePosition - self.start_point
    -- LOG("OnDrag", dt.x, dt.y)
    if U.Input.touchCount == 2 then
        local touch_dis = (U.Input.touches[0].position - U.Input.touches[1].position).magnitude
        if not self.touch_dis then
            self.touch_dis = touch_dis
            self.mode = 2
        else
            -- self.camera.transform.position = self.camera_pos + U.Vector3(0,touch_dis - self.touch_dis,0) * 0.05
            local pos = self.camera_pos + self.camera_forward * (touch_dis - self.touch_dis) * 0.05
            if pos.y < 1 then
                local dy = (1  - self.camera_pos.y) /  self.camera_forward.y 
                pos = self.camera_pos + self.camera_forward * dy
            end
            self.camera.transform.position = pos
            
            
        end
    else
        if self.touch_dis then
            self.touch_dis = nil
            self.camera_pos = self.camera.transform.position
        elseif self.mode == 1 then 
            self.camera.transform.position = self.camera_pos + U.Vector3(dt.x,0,dt.y) * (-0.005 * self.camera_pos.y)
        end
    end
    
end