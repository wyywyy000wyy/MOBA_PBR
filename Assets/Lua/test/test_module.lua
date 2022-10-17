module("test_module", package.seeall)
value = value or 4

function print_ac()
    logger.print("print_ac=" .. value)
end

test_class = class(test_class, function(self)
    self.name = "aaa"
    self.value = 5
end)

function test_class:print_test()
    logger.print("print_gooood=" ,self.name, self.value)
    self.value = self.value + 1
end

