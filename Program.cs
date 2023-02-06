// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


value a = new value(1.5, null, "a");
value b = new value(2.0, null, "b");
value c = new value(3.0, null, "b");

value d=a+b*c;

Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);
Console.WriteLine(d.grad);

d.grad=2.0;
d.backward();


Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);
Console.WriteLine(d.grad);

// Console.WriteLine(c.data);