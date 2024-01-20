local file_system = class("file_system", function(self)
end)

local IsFileExist = T.LuaFilePicker.IsFileExist
local GetFilePath = T.LuaFilePicker.GetFilePath
local FileWriteTime = T.LuaFilePicker.FileWriteTime
local GetFolderFiles = T.LuaFilePicker.GetFolderFiles


function file_system.exsit_file(path)
    return IsFileExist(path)
end

function file_system.get_file_path(path)
    return GetFilePath(path)
end
 
function file_system.get_file_time(path)
    return FileWriteTime(path)
end

function file_system.get_files(path, recursive)
    return GetFolderFiles(path, recursive)
end