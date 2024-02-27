

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

-- local player1 = PlayerType("player1", 1)
-- local player2 = PlayerType("player2", 2)

-- LOG("player1", player1)
-- LOG("player2", tostring(player2))
-- LOG("Int(5)", Int(5))

local guoguo = PlayerType("player_guoguo", 55)

local t = task
:persistent_save("Player", 1, guoguo)
:persistent_load("Player", 1)
:call(function(t, tv1, tv2)
    local result = t:pr()
    LOG("persistent_load_call__", result)
end, 22, 33)
:excute()

local a = 1
a  =2
