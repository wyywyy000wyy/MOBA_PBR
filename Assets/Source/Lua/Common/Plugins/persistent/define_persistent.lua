return {
    action = {
        action_def("persistent_load", {
            PD(String, "table"),
            PD(Key, "key"),
        }),
        action_def("persistent_save", {
            PD(String, "table"),
            PD(Key, "key"),
            PD(Struct, "data")
        }),
    },
    event = {

    },
}