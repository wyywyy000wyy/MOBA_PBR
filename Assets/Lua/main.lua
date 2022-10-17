

require("core.module")
require("core.class")
require("core.serpent")
require("core.logger")
require("global_require")
require("core.ys_loader")

logger.print("DrawTest Lua loaded ")
-- require("TestSwitchDraw3")

local wait_frame = 0

local cor = coroutine.create(function()
    for i = 1, wait_frame do
        coroutine.yield()
    end
    -- require("test.TestSwitchDraw1")
    -- require("test.TestSwitchDraw2")
    -- require("test.TestSwitchDraw3")
end)

for i = 1, wait_frame do
    coroutine.resume(cor)
end

coroutine.resume(cor)
logger.print(coroutine.status(cor)) -- suspended
logger.print(coroutine.running())







logger.print("do main2 ".. CS.UnityEngine.Application.persistentDataPath)

local path2 = CS.UnityEngine.Application.persistentDataPath .. "/Lua/Main.lua"
local path = "/storage/emulated/0/Pictures/Lua/Main.lua"

-- local file = io.open(path)

-- local cur = CS.DrawTest.FileWriteTime(path)



require("test.test_module")
require("test.TestSwitchDraw3")


