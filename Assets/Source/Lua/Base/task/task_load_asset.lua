local task_load_asset = class("task_load_asset", T.task, function(self, asset_path)
    T.task._ctor(self)
    self._asset_path = asset_path
end)

function task_load_asset:on_execute()
    T.YSLoader.InstantiateAsync(self._asset_path, function(asset)
        self._asset = asset
        self:finish()
    end)
end