

-- Task()
-- :persistent_load("table", "key")
-- :excute()

-- sss = "common/framework/type/string"
-- sss_folder = T.file_system.get_folder(sss)
-- LOG("sss", sss,  "folder", sss_folder)

local PlayerType = StructDef("Player", 
    PD(String, "name", "default_name"),
    PD(Int, "Lv", 1)
)

local guoguo = PlayerType("player_guoguo", 55)

task
:persistent_save("Player", 1, guoguo)
:persistent_load("Player", 1)
:call(function(t, tv1, tv2)
    local result = t:pr()
    LOG("persistent_load_call__", result)
end, 22, 33)
:excute()

local guoguo_data = cmsgpack.pack(guoguo)
local guoguo_table = cmsgpack.unpack(guoguo_data)
local a = 1
a  =2
