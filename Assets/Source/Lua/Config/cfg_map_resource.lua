cfg_map_resource = {
    [1] = { ID = 1, path="Map/MapContainer/MapBlock_0.prefab"},
    [2] = { ID = 2, path="Map/MapContainer/MapBlock_1.prefab"},
    [3] = { ID = 3, path="Map/MapContainer/MapBlock_2.prefab"},
    [4] = { ID = 4, path="Map/MapContainer/MapBlock_3.prefab"},
}


local MapResourceCenter = CS.MapResourceCenter

for k, v in ipairs(cfg_map_resource) do
    MapResourceCenter.RegisterConfig(v.ID, v.path)
end
