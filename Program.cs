// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


value a = new value(1.5, null, "a");
value b = new value(2.0, null, "b");
value c = new value(3.0, null, "b");
value d=a+b*c;
value e=d^(new value(2.0,null, "2"));
value f=value.relu(e);   

Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);
Console.WriteLine(d.grad);
Console.WriteLine(e.grad);
Console.WriteLine(f.grad);


f.grad=10.0;
f.backward();


Console.WriteLine(a.grad);
Console.WriteLine(b.grad);
Console.WriteLine(c.grad);
Console.WriteLine(d.grad);
Console.WriteLine(e.grad);
Console.WriteLine(f.grad);
