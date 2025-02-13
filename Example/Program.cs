using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;
using SharpGrad.Operator;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 4);
        var v = DataSet.GetDataSet(400);
        if (v.Count == 0)
            throw new ArgumentException("Empty dataset.");

        MLP<float> cerebrin = new(2, 8, 1);

        int epochs = 1000;

        DataSet.Data[] preds = new DataSet.Data[v.Count];

        Dimension batch = new(v.Count);
        float lr = 1e-5f;
        // List of input data
        var x1 = v.Select(d => (float)d.X[0]).ToArray();
        var x2 = v.Select(d => (float)d.X[1]).ToArray();
        Variable<float>[] X = [new(x1, [batch], "X0"), new(x2, [batch], "X1")];
        // List of ground truth data
        var ygt = v.Select(d => (float)d.Y[0]).ToArray();
        Variable<float>[] Ygt = [new(ygt, [batch], "Ygt")];

        // Build execution expression graph (no computation done here)
        Value<float>[] Y = cerebrin.Forward(X);
        NariOpValue<float> loss = Loss.MSE(Y, Ygt);
        loss.IsOutput = true;

        // Training loop
        float minLoss = float.MaxValue;
        float lastLoss = 0;
        int wait = 128;
        for (int i = 0; i < epochs; i++)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"LR: {lr:E2} | Epoch: {i} / {epochs}");
            // Forward and backward pass
            loss.Forward();
            loss.Backward();

            // Build prediction data
            foreach (Dimdices dimdices in new Dimdexer(Y[0].Shape))
            {
                int j = dimdices[batch];
                float d = Y[0].Data[j];
                int val = Math.Abs(d - 1) < Math.Abs(d - 2) ? 1 : 2;
                preds[j] = new(v[j].X, [val]);
            }

            // Update weights
            cerebrin.Step(lr);
            // Reset gradients
            loss.ResetGradient();

            // Print loss and scatter plot
            Console.WriteLine($"Loss: {loss.Data[0]:E3} / {minLoss:E3}");
            DataSet.Scatter(v, preds);
            if (minLoss > loss.Data[0])
            {
                minLoss = loss.Data[0];
            }
            if (lastLoss == loss.Data[0])
            {
                wait--;
                if (wait <= 0)
                {
                    Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 15);
                    Console.WriteLine("Final loss: " + loss.Data[0]);
                    Console.WriteLine("Last epoch: " + i);
                    Console.WriteLine("Loss not progress. Stopping training...");
                    break;
                }
            }
            else
            {
                lastLoss = loss.Data[0];
                wait = 128;
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
