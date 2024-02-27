local service_persistent = plugin_service_def("service_persistent")

function service_persistent:init()
    -- require("plugins/persistent/service/service_persistent_impl_filesystem")
    lrequire("service_persistent_impl_filesystem")
    self._impl = T.service_persistent_impl_filesystem()
    self._impl:init()
end

function service_persistent:persistent_load(action, table, key)
    LOG("service_persistent:persistent_load()", table, key)
    local result = self._impl:load(table, key)
    action:finish(result)
    -- return self._impl:load(action.table, action.key)
end

function service_persistent:persistent_save(action, table, key, data)
    self._impl:save_data(table, key, data)
    action:finish()
end
