// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


value a = new value(1.5, null, "a");
value b = new value(2.0, null, "b");

value c=a+b;

Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);

c.grad=2.0;
c.backward();


Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);

// Console.WriteLine(c.data);