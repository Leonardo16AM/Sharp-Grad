
using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;

var v = DataSet.GetDataSet(400);
Console.WriteLine("Dataset:");
Scatter(v);



MLP cerebrin = new(2, 8, 1);

List<Value> X = new()
{
    new Value(v[0].X[0], "a"),
    new Value(v[0].X[1], "b")
};
List<Value> Y = cerebrin.Forward(X);


Console.WriteLine(Y[0].Data);



int epochs = 1000;
double lr = 0.0000000001;

double lastLoss = double.MaxValue;

for (int i = 0; i < epochs; i++)
{
    Console.WriteLine("Epoch: " + i);
    Value loss = new(0.0, "loss");
    List<DataSet.Data> preds = new();

    for (int j = 0; j < v.Count; j++)
    {
        X = new List<Value>
        {
            new(v[j].X[0], "a"),
            new(v[j].X[1], "b")
        };
        Y = cerebrin.Forward(X);
        List<Value> Ygt = new()
        {
            new(v[j].Y[0], "c")
        };
        var nl = loss + MSE(Y, Ygt);
        loss = nl;

        int val;
        if (Math.Abs(Y[0].Data - 1) < Math.Abs(Y[0].Data - 2))
        {
            val = 1;
        }
        else
        {
            val = 2;
        }
        DataSet.Data nd = new(v[j].X, new List<int> { val });
        preds.Add(nd);
    }

    loss.Grad = 1.0;
    loss.Backpropagate();
    foreach (Layer l in cerebrin.Layers)
    {
        foreach (Neuron n in l.Neurons)
        {
            foreach (Value w in n.Weights)
            {
                // Console.WriteLine(w.data);
                w.Data -= lr * w.Grad;
            }
            n.Biai.Data -= lr * n.Biai.Grad;
        }
    }

    loss.ResetGrad();

    Console.WriteLine("Loss: " + loss.Data);
    Scatter(preds);
    if (lastLoss > loss.Data)
    {
        lastLoss = loss.Data;
    }
    else
    {
        Console.WriteLine("Loss is increasing. Stopping training...");
        break;
    }
}




Value MSE(List<Value> Y, List<Value> Y_hat)
{
    Value loss = new(0.0, "loss");
    for (int i = 0; i < Y.Count; i++)
    {
        var nl = loss + ((Y[i] - Y_hat[i]) ^ (new Value(2.0, "w")));
        loss = nl;
    }
    return loss;
}














void Scatter(List<DataSet.Data> v)
{
    Console.Clear();
    Console.WriteLine("\x1b[3J");

    int[,] mat = new int[300, 300];
    for (int i = 0; i < v.Count; i++)
    {
        mat[v[i].X[0] + 15, v[i].X[1] + 15] = v[i].Y[0];
    }
    Console.Write("╔");
    for (int i = 0; i < 30; i++)
    {
        Console.Write("═");
    }
    Console.WriteLine("╗");
    for (int i = 0; i < 30; i++)
    {
        Console.Write("║");
        for (int j = 0; j < 30; j++)
        {
            if (mat[i, j] == 0)
            {
                Console.Write(" ");
            }
            if (mat[i, j] == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("o");
            }
            if (mat[i, j] == 2)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("o");
            }
            Console.ResetColor();
        }
        Console.WriteLine("║");
    }

    Console.Write("╚");
    for (int i = 0; i < 30; i++)
    {
        Console.Write("═");
    }
    Console.WriteLine("╝");
}

static class DataSet
{

    public class Data
    {
        public List<int> X;
        public List<int> Y;

        public Data(List<int> X, List<int> Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public static List<Data> GetDataSet(int n)
    {
        var rand = new Random();
        List<Data> dataset = new();
        for (int i = 0; i < n; i++)
        {
            List<int> X = new();
            List<int> Y = new();
            X.Add(rand.Next(-15, 15));
            X.Add(rand.Next(-15, 15));
            int x = X[0];
            int y = X[1];
            if (y > 2 * x + 5)
                Y.Add(1);
            else
                Y.Add(2);

            dataset.Add(new(X, Y));
        }
        return dataset;
    }
}







//Value a = new Value(1.5,"a");
//Value b = new Value(2.0,"b");
//Value c = new Value(6.0,"b");

// value d=(a+b*c);
// value e=d/(new value(2.0,"2"));
// value f=e^(new value(2.0,"2"));
// value g=f.relu();   

// g.grad=1.0;
// g.backpropagate();

// Console.WriteLine(a.grad);
// Console.WriteLine(b.grad);
// Console.WriteLine(c.grad);

// value j= new value(0.5,"j");
// value k= j.tanh();

// k.grad=1.0;
// k.backpropagate();
// Console.WriteLine(j.grad);
// Console.WriteLine(k.data);


// MLP cerebrin = new MLP(4,new List<int>{4,16,16,1});

// List<value> X = new List<value>();
// X.Add(new value(1.0,"x1"));
// X.Add(new value(2.0,"x2"));
// X.Add(new value(3.0,"x3"));
// X.Add(new value(4.0,"x4"));

// List<value> Y = cerebrin.forward(X);
// Console.WriteLine(Y[0].data);