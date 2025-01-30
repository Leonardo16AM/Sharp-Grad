using SharpGrad;
using SharpGrad.Activation;
using SharpGrad.DifEngine;
using SharpGrad.NN;
using SharpGrad.Operator;
using System.Diagnostics;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 4);
        var v = DataSet.GetDataSet(400);

        MLP<float> cerebrin = new(2, 8, 1);

        int epochs = 1000;
        float lr = 1e-4f;

        DataSet.Data[] preds = new DataSet.Data[v.Count];

        // List of input data
        Variable<float>[][] X = new Variable<float>[v.Count][];
        // List of ground truth data
        Variable<float>[][] Ygt = new Variable<float>[v.Count][];
        // List of predicted data
        Value<float>[][] Y = new Value<float>[v.Count][];


        // Build execution expression graph (no computation done here)
        NariOpValue<float>? loss = null;
        for (int i = 0; i < v.Count; i++)
        {
            X[i] = [v[i].X[0], v[i].X[1]];
            Ygt[i] = [v[i].Y[0]];
            Y[i] = cerebrin.Forward(X[i]);
            if (loss is null)
                loss = Loss.MSE(Y[i], Ygt[i]) / v.Count;
            else
                loss += Loss.MSE(Y[i], Ygt[i]) / v.Count;
        }
        if(loss is null)
            throw new Exception("No loss function defined.");
        else
            loss.IsOutput = true;

        // Training loop
        float lastLoss = float.MaxValue;
        for (int i = 0; i < epochs; i++)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"LR: {lr} | Epoch: {i} / {epochs}");

            // Forward and backward pass
            //loss.ForwardLambda();
            loss.BackwardLambda();

            for (int j = 0; j < Y.Length; j++)
            {
                int val = Math.Abs(Y[j][0].Data - 1) < Math.Abs(Y[j][0].Data - 2) ? 1 : 2;
                preds[j] = new(v[j].X, [val]);
            }

            cerebrin.Step(lr);
            loss.ResetGradient();

            Console.WriteLine("Loss: " + loss.Data);
            DataSet.Scatter(v, preds);
            if (lastLoss > loss.Data)
            {
                lastLoss = loss.Data;
            }
            else
            {
                Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 15);
                Console.WriteLine("Final loss: " + loss.Data);
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
