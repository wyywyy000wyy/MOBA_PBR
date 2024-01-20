-- module("hot_require", package.seeall)
local table_insert = table.insert
local file_system = T.file_system
local get_file_path = T.file_system.get_file_path
local get_file_time = T.file_system.get_file_time
local get_files = T.file_system.get_files
g_require = g_require or require
local prequire = g_require
hot_require = {}

local search_paths = {
    "Common/",
    "",
}

local require_file = class("require_file", function(self, modname)
    self.modname = modname
    self.path = modname
    self.time = 0
    self.file_count = 0

    local files = {}
    for _, path in ipairs(search_paths) do
        local filepath = path .. self.path .. ".lua"
        table_insert(files, filepath)
    end
    self.files = files
    -- self.filePaths = {}
end)



function require_file:dirty()
    local file_count = self:get_files_count()
    if file_count ~= self.file_count then
        return true
    end
    for _, path in ipairs(self.files) do
        if file_system.exsit_file(path) then
            local require_path = get_file_path(string.sub(path, 1, -5))
            local file_time = file_system.get_file_time(require_path)
            if file_time > self.time then
                return true
            end
        end
    end
    return false
end

function require_file:get_files_count()
    local file_count = 0
    for _, path in ipairs(self.files) do
        if file_system.exsit_file(path) then
            file_count = file_count + 1
        end
    end
    return file_count
end


function require_file:require()
    if not self:dirty() then
        return
    end
    self.file_count = 0
    local first = self.time == 0
    for i, path in ipairs(self.files) do
        if file_system.exsit_file(path) then
            -- if not require_path then
            --     self.filePaths[i] = require_path
            -- end
            local require_path = get_file_path(string.sub(path, 1, -5))
            self.time = math.max(self.time, get_file_time(require_path))
            self.file_count = self.file_count + 1
            if not first then
                ELOG("GGGGGGGG....... 热更了文件：" .. path)
            end
            dofile(require_path)
        end
    end
end

local init_map = {}
local init_list = {}


require = function(modname)
    local require_info = init_map[modname]
    if not require_info then
        require_info = require_file(modname)
        init_map[modname] = require_info
        table.insert(init_list, require_info)
    end
    require_info:require()
end

function require_folder(folder, recursive)

    for _, path in ipairs(search_paths) do
        local filepath = path .. folder
        local files = get_files(filepath, recursive)
        local len = files.Count
        for i = 0, len - 1 do
            local filepath = filepath .. "/" .. files[i]
            -- ELOG("require_folder", filepath)
            require(filepath)
        end
    end

    -- local files = get_files(folder, recursive)

    -- local len = files.Count
    -- for i = 0, len - 1 do
    --     local filepath = folder .. "/" .. files[i]
    --     -- ELOG("require_folder", filepath)
    --     require(filepath)
    -- end
end

function hot_require.force_require(modname)
    hot_require.rrquire_all(modname)
end

function hot_require.rrquire_all(modname)
    ELOG("GGGGGGGG....... lua 热更")
    for _, info in ipairs(init_list) do
        info:require()
    end
end