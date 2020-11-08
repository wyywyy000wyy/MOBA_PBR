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
    -- self.scene_script_root = U.GameObject.Find("SceneScriptRoot")

    self.map_controller = MapController.new(self.scene_script_root)
    
    -- self.load_model_arr = {
    --     "Entity/H_Shiliu2.prefab",
    --     "Entity/gebulin01.prefab",
    --     "Entity/hulu.prefab",
    --     "Entity/yezhu02.prefab",
    -- }

    -- self.loaded_models = {}

    -- local count = 0
    -- for i, path in ipairs(self.load_model_arr) do
    --     U.XLoader.LoadAsync(path, typeof(U.GameObject),function(go)
    --         self.loaded_models[i] = go
    --         local ss = 0.2
    --         go.transform.localScale = U.Vector3(ss,ss,ss)
    --         count = count + 1
    --         if count >= #self.load_model_arr then
    --             self:OnEntityLoaded()
    --         end
    --     end)
    -- end

    -- CS.MapResourceCenter.Start()
    local count = 0
    for _, v in pairs (cfg_map_resource) do
        CS.MapResourceCenter.PreLoad(v.ID, function()
            count = count + 1
            if count == #cfg_map_resource then
                self:OnEntityLoaded()
            end
        end)
    end


end

function Map:OnEntityLoaded()
    -- local scene_width = 100
    -- local start_pos = U.Vector3(-scene_width/2,0,-scene_width/2)
    -- local n_count = 25

    -- local dx = U.Vector3(scene_width / 20, 0, 0)
    -- local dz = U.Vector3(0, 0, scene_width / 20)
    -- for z = 0, n_count do
    --     for x = 0, n_count do
    --         local tpos = start_pos + dz * z + dx * x
    --         local model = self.loaded_models[math.random(1, #self.loaded_models)]
    --         U.GameObject.Instantiate(model, tpos, U.Quaternion.identity)
    --     end
    -- end

    CS.Map.Instance:StartMap()
end