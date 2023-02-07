// value a = new value(1.5,"a");
// value b = new value(2.0,"b");
// value c = new value(6.0,"b");
// value d=(a+b*c);
// value e=d/(new value(2.0,"2"));
// value f=e^(new value(2.0,"2"));
// value g=value.tanh(f);   


// g.grad=1.0;
// g.backpropagate();


// Console.WriteLine(a.grad);
// Console.WriteLine(b.grad);
// Console.WriteLine(c.grad);
// Console.WriteLine(d.grad);
// Console.WriteLine(e.grad);
// Console.WriteLine(f.grad);
// Console.WriteLine(g.grad);

value j= new value(0.5,"j");
value k= j.tanh();
k.grad=1.0;
k.backpropagate();
Console.WriteLine(j.grad);
Console.WriteLine(k.data);


// MLP cerebrin = new MLP(4,new List<int>{4,16,16,1});

// List<value> X = new List<value>();
// X.Add(new value(1.0,"x1"));
// X.Add(new value(2.0,"x2"));
// X.Add(new value(3.0,"x3"));
// X.Add(new value(4.0,"x4"));

// List<value> Y = cerebrin.forward(X);
// Console.WriteLine(Y[0].data);