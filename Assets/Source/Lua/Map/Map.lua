Map = class {

}

function Map:ctor()
    self.scene_name_ = nil
    self.scene_loaded_ = false
    self.map_controller = nil
end

function Map:LoadMap(scene_name)
    self.scene_name_ = scene_name
    coroutine.wrap(function()
        self.scene_loaded_ = false

        LOG("Map:LoadMap wrap", scene_name)
        Yield(U.XLoader.LoadSceneAsync(self.scene_name_,function(done)
            if not done then
                ELOG("LoadSceneAsync ",self.scene_name_," failed!")
                return
            end
        end))
        LOG("Map:LoadMap scene_loaded_", scene_name)
        self.scene_loaded_ = true
        self:OnMapLoaded()
    end)()

end

function Map:OnMapLoaded()
    self.scene_script_root = U.GameObject.Find("SceneScriptRoot")

    self.map_controller = MapController.new(self.scene_script_root)
    
end