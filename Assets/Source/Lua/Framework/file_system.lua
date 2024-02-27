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

function file_system.get_folder(path)
    local paths = string.split(path, "/")
    local folder = table.concat(paths, "/", 1, #paths - 1)
    return folder
end

function file_system.read_file(path)
    local file = io.open(path, "rb")
    if not file then
        return nil
    end
    local content = file:read("*all")
    file:close()
    return content
end

function file_system.write_file(path, content)
    local file = io.open(path, "wb")
    if not file then
        return false
    end
    file:write(content)
    file:close()
    return true
end