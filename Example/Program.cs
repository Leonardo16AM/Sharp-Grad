using SharpGrad.Activation;
using SharpGrad.DifEngine;
using SharpGrad.NN;
using System.Diagnostics;

internal class Program
{
    public static Random Rnd = new();
    #region TEST
    // TEST START
    public static void Add()
    {
        Variable<float> a = new(1.5f, "a");
        Variable<float> b = new(2.0f, "b");
        var c = a + b;
        var cFunc = c.ForwardLambda;
        cFunc();
        Debug.Assert(c.Data == 3.5f);

        a.Data = 2.0f;
        b.Data = 3.0f;
        cFunc();
        Debug.Assert(c.Data == 5.0f);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            var bData = (float)Rnd.NextDouble();
            b.Data = bData;
            cFunc();
            Debug.Assert(c.Data == aData + bData);
        }
    }

    public static void Sub()
    {
        Variable<float> a = new(1.5f, "a");
        Variable<float> b = new(2.0f, "b");
        var c = a - b;
        var cFunc = c.ForwardLambda;
        cFunc();
        Debug.Assert(c.Data == -0.5f);

        a.Data = 2.0f;
        b.Data = 3.0f;
        cFunc();
        Debug.Assert(c.Data == -1.0f);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            var bData = (float)Rnd.NextDouble();
            b.Data = bData;
            cFunc();
            Debug.Assert(c.Data == aData - bData);
        }
    }

    public static void Mul()
    {
        Variable<float> a = new(1.5f, "a");
        Variable<float> b = new(2.0f, "b");
        var c = a * b;
        var cFunc = c.ForwardLambda;
        cFunc();
        Debug.Assert(c.Data == 3.0f);

        a.Data = 2.0f;
        b.Data = 3.0f;
        cFunc();
        Debug.Assert(c.Data == 6.0f);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            var bData = (float)Rnd.NextDouble();
            b.Data = bData;
            cFunc();
            Debug.Assert(c.Data == aData * bData);
        }
    }

    public static void Div()
    {
        Variable<float> a = new(1.5f, "a");
        Variable<float> b = new(2.0f, "b");
        var c = a / b;
        var cFunc = c.ForwardLambda;
        cFunc();
        Debug.Assert(c.Data == 0.75f);

        a.Data = 2.0f;
        b.Data = 3.0f;
        cFunc();
        Debug.Assert(c.Data == 2.0f / 3.0f);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            var bData = (float)Rnd.NextDouble();
            b.Data = bData;
            cFunc();
            Debug.Assert(c.Data == aData / bData);
        }
    }

    public static void Pow()
    {
        Variable<float> a = new(1.5f, "a");
        Variable<float> b = new(2.0f, "b");
        var c = a.Pow(b);
        var cFunc = c.ForwardLambda;
        cFunc();
        Debug.Assert(c.Data == Math.Pow(1.5, 2.0));

        a.Data = 2.0f;
        b.Data = 3.0f;
        cFunc();
        Debug.Assert(c.Data == Math.Pow(2.0, 3.0));

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            var bData = (float)Rnd.NextDouble();
            b.Data = bData;
            cFunc();
            var r = (float)Math.Pow(aData, bData);
            Debug.Assert(c.Data == r);
        }
    }

    public static void Sigmoid()
    {
        Variable<float> a = new(1.5f, "a");
        var c = a.Sigmoid();
        var cFunc = c.ForwardLambda;
        cFunc();
        var r = 1.0f / (1.0f + (float)Math.Exp(-1.5f));
        Debug.Assert(c.Data == r);

        a.Data = 2.0f;
        cFunc();
        r = 1.0f / (1.0f + (float)Math.Exp(-2.0f));
        Debug.Assert(c.Data == r);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            cFunc();
            r = 1.0f / (1.0f + (float)Math.Exp(-aData));
            Debug.Assert(c.Data == r);
        }
    }

    public static void Tanh()
    {
        Variable<float> a = new(1.5f, "a");
        var c = a.Tanh();
        var cFunc = c.ForwardLambda;
        cFunc();
        var r = (float)Math.Tanh(1.5f);
        Debug.Assert(c.Data == r);

        a.Data = 2.0f;
        cFunc();
        r = (float)Math.Tanh(2.0f);
        Debug.Assert(c.Data == r);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            cFunc();
            r = (float)Math.Tanh(aData);
            Debug.Assert(c.Data == r);
        }
    }

    public static void ReLU()
    {
        Variable<float> a = new(1.5f, "a");
        var c = a.ReLU();
        var cFunc = c.ForwardLambda;
        cFunc();
        var r = (float)Math.Max(0, 1.5f);
        Debug.Assert(c.Data == r);

        a.Data = -2.0f;
        cFunc();
        r = (float)Math.Max(0, -2.0f);
        Debug.Assert(c.Data == r);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            cFunc();
            r = (float)Math.Max(0, aData);
            Debug.Assert(c.Data == r);
        }
    }

    public static void LeakyReLU()
    {
        Variable<float> a = new(1.5f, "a");
        var c = a.LeakyReLU(0.1f);
        var cFunc = c.ForwardLambda;
        cFunc();
        var r = Math.Max(0.1f * 1.5f, 1.5f);
        Debug.Assert(c.Data == r);

        a.Data = -2.0f;
        cFunc();
        r = Math.Max(0.1f * -2.0f, -2.0f);
        Debug.Assert(c.Data == r);

        for (int i = 0; i < 10; i++)
        {
            var aData = (float)Rnd.NextDouble();
            a.Data = aData;
            cFunc();
            r = Math.Max(0.1f * aData, aData);
            Debug.Assert(c.Data == r);
        }
    }

    // END TEST
    #endregion

    private static void Main(string[] args)
    {
        //// TEST /////
        //Add();
        //Sub();
        //Mul();
        //Div();
        //Pow();
        //Sigmoid();
        //Tanh();
        //ReLU();
        //LeakyReLU();
        ///////////////////

        Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 4);
        var v = DataSet.GetDataSet(400);

        MLP<float> cerebrin = new(2, 8, 1);

        int epochs = 1000;
        float lr = 1e-9f;

        float lastLoss = float.MaxValue;

        for (int i = 0; i < epochs; i++)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"LR: {lr} | Epoch: {i} / {epochs}");
            List<DataSet.Data> preds = [];



            Variable<float>[] X = [
                v[0].X[0],
                v[0].X[1]
            ];
            Variable<float>[] Ygt = [
                v[0].Y[0]
            ];

            IReadOnlyList<Value<float>> Y = cerebrin.Forward(X);
            var loss = Loss.MSE(cerebrin.Forward(X), Ygt);

            Func<float> mse = loss.ForwardLambda;
            float l = loss.ForwardLambda();
            loss.Backpropagate();

            int val = Math.Abs(Y[0].Data - 1) < Math.Abs(Y[0].Data - 2) ? 1 : 2;
            preds.Add(new(v[0].X, [val]));



            for (int j = 0; j < v.Count; j++)
            {
                X[0].Data = v[j].X[0];
                X[1].Data = v[j].X[1];
                Ygt[0] = v[j].Y[0];

                l += loss.ForwardLambda();
                loss.Backpropagate();

                val = Math.Abs(Y[0].Data - 1) < Math.Abs(Y[0].Data - 2) ? 1 : 2;
                preds.Add(new(v[j].X, [val]));
            }

            cerebrin.Step(lr);

            Console.WriteLine("Loss: " + l);
            DataSet.Scatter(v, preds);
            if (lastLoss > l)
            {
                lastLoss = l;
            }
            else
            {
                Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 15);
                Console.WriteLine("Final loss: " + l);
                Console.WriteLine("Last epoch: " + i);
                Console.WriteLine("Loss is increasing. Stopping training...");
                break;
            }
        }
    }
}












// Value<float> a = new Value<float>(1.5f,"a");
// Value<float> b = new Value<float>(2.0f,"b");
// Value<float> c = new Value<float>(6.0f,"b");

// Value<float> d=(a+b*c);
// Value<float> e=d/(new Value<float>(2.0f,"2"));
// Value<float> f=e.Pow(new Value<float>(2.0f,"2"));
// Value<float> g=f.ReLU();   

// g.Grad=1.0f;
// g.Backpropagate();

// Console.WriteLine(a.Grad);
// Console.WriteLine(b.Grad);
// Console.WriteLine(c.Grad);

// Value<float> j= new Value<float>(0.5f,"j");
// Value<float> k= j.Tanh();
// Value<float> l= k.Sigmoid();
// Value<float> m= l.LeakyReLU(1.0f);
// m.Grad=1.0f;
// m.Backpropagate();
// Console.WriteLine(j.Grad);
// Console.WriteLine(m.Data);


/***
Tested with torch:

import torch

a = torch.tensor(1.5, requires_grad=True)
b = torch.tensor(2.0, requires_grad=True)
c = torch.tensor(6.0, requires_grad=True)

d = a + b * c
e = d / 2.0
f = e ** 2
g = torch.relu(f)

g.backward()

print("Gradiente de a:", a.grad)
print("Gradiente de b:", b.grad)
print("Gradiente de c:", c.grad)



def custom_leaky_relu(x, negative_slope=1.0):
    return torch.where(x > 0, x, negative_slope * x)


j = torch.tensor(0.5, requires_grad=True)
k = torch.tanh(j)
l= torch.sigmoid(k)
m = custom_leaky_relu(l, negative_slope=1.0)

m.backward()

print(j.grad)
print( m.item())
***/
