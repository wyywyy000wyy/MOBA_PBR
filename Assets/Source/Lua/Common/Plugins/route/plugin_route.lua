local plugin_route = plugin_def("plugin_route")

function plugin_route:init()
    self.route_table = {}
    self.work_table = {}
end