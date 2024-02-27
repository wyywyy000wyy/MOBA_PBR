task_def("call", function(task_cur, func, ...)
    func(task_cur, ...)
    task_cur:finish()
end)


