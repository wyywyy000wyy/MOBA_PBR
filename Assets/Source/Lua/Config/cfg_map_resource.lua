cfg_map_resource = {
    [1] = { ID = 1, path="Map/MapContainer/MapBlock_0.prefab"},
    [2] = { ID = 2, path="Map/MapContainer/MapBlock_1.prefab"},
    [3] = { ID = 3, path="Map/MapContainer/MapBlock_2.prefab"},
    [4] = { ID = 4, path="Map/MapContainer/MapBlock_3.prefab"},
    [5] = { ID = 5, path="Map/MapContainer/MapBlock_4.prefab"},
    [6] = { ID = 6, path="Map/MapContainer/MapBlock_5.prefab"},
    [7] = { ID = 7, path="Map/MapContainer/MapBlock_6.prefab"},
    [8] = { ID = 8, path="Map/MapContainer/MapBlock_7.prefab"},

    [1001] = { ID = 1001, path="Entity/Npc/npc_1.prefab"},
    [1002] = { ID = 1002, path="Entity/Npc/npc_2.prefab"},
    [1003] = { ID = 1003, path="Entity/Npc/npc_3.prefab"},
    [1004] = { ID = 1004, path="Entity/Npc/npc_4.prefab"},
    [1005] = { ID = 1005, path="Entity/Npc/npc_3.prefab"},
    [1006] = { ID = 1006, path="Entity/Npc/npc_3.prefab"},
    [1007] = { ID = 1007, path="Entity/Npc/npc_4.prefab"},    
    [1008] = { ID = 1008, path="Entity/Npc/npc_4.prefab"},
    [1009] = { ID = 1009, path="Entity/Npc/npc_3.prefab"},
    [1010] = { ID = 1010, path="Entity/Npc/npc_4.prefab"},

    [1001001] = { ID = 1001001, path="Entity/gebulin01.prefab"},
    [1001002] = { ID = 1001002, path="Entity/npc_1_2.prefab"},
    [1001003] = { ID = 1001003, path="Entity/npc_1_3.prefab"},
    [1002001] = { ID = 1002001, path="Entity/H_Shiliu2.prefab"},
    [1002002] = { ID = 1002002, path="Entity/npc_2_2.prefab"},
    [1003001] = { ID = 1003001, path="Entity/hulu.prefab"},
    [1004001] = { ID = 1004001, path="Entity/yezhu02.prefab"},

}


local MapResourceCenter = CS.MapResourceCenter

for k, v in pairs(cfg_map_resource) do
    MapResourceCenter.RegisterConfig(v.ID, v.path)
end
