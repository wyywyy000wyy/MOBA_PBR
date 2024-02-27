local service_persistent_impl_filesystem = class("service_persistent_impl_filesystem", function(self)
end)
local db_file_name = "persistent.db"

function service_persistent_impl_filesystem:init()
    local db_str = T.file_system.read_file(db_file_name)
    local db
    if db_str then
        db = cjson.decode(db_str)
    end
    db = db or {}
    self._db = db
end

function service_persistent_impl_filesystem:save()
    local jsonStr = cjson.encode(self._db)
    T.file_system.write_file(db_file_name, jsonStr)
end

function service_persistent_impl_filesystem:load(table, key)
    local data = self._db[table]
    if data then
        return data[key]
    end
end

function service_persistent_impl_filesystem:save_data(table, key, data)
    local table_data = self._db[table]
    if not table_data then
        table_data = {}
        self._db[table] = table_data
    end
    table_data[key] = data
    self:save()
end