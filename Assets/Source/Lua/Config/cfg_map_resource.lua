cfg_map_resource = {
    [1] = { ID = 1, path="Map/MapContainer/MapBlock_0.prefab"},
    [2] = { ID = 2, path="Map/MapContainer/MapBlock_1.prefab"},
    [3] = { ID = 3, path="Map/MapContainer/MapBlock_2.prefab"},
    [4] = { ID = 4, path="Map/MapContainer/MapBlock_3.prefab"},

    [1001] = { ID = 1001, path="Entity/Npc/npc_1.prefab"},
    [1002] = { ID = 1002, path="Entity/Npc/npc_2.prefab"},
    [1003] = { ID = 1003, path="Entity/Npc/npc_3.prefab"},
    [1004] = { ID = 1004, path="Entity/Npc/npc_4.prefab"},

    [1001001] = { ID = 1001001, path="Entity/gebulin01.prefab"},
    [1002001] = { ID = 1002001, path="Entity/H_Shiliu2.prefab"},
    [1003001] = { ID = 1003001, path="Entity/hulu.prefab"},
    [1004001] = { ID = 1004001, path="Entity/yezhu02.prefab"},

}


local MapResourceCenter = CS.MapResourceCenter

for k, v in pairs(cfg_map_resource) do
    MapResourceCenter.RegisterConfig(v.ID, v.path)
end
