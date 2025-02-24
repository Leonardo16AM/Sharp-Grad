using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;
using SharpGrad.Operators;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.SetWindowSize(DataSet.N * 2 + 4, DataSet.N + 4);

        Dimension batch = new(nameof(batch), 400);
        var v = DataSet.GetDataSet(batch.Size);

        Dimension input = new(nameof(input), 2);
        Dimension hidden = new(nameof(hidden), 8);
        Dimension output = Dimension.Scalar;
        MLP<float> cerebrin = new([input, hidden, output]);

        int epochs = 1000;

        DataSet.Data[] preds = new DataSet.Data[batch.Size];

        float lr = 1e-4f;
        // List of input data
        Variable<float> X = new([batch, input], "X");
        foreach (Dimdices dimdices in new Dimdexer(X.Shape))
        {
            X[dimdices] = v[dimdices[batch]].X[dimdices[input]];
        }

        // List of ground truth data
        var ygt = v.Select(d => (float)d.Y[0]).ToArray();
        Variable<float> Ygt = new(ygt, [output, batch], "Ygt");

        // Build execution expression graph (no computation done here)
        Value<float> Y = cerebrin.Forward(X);
        NariOperation<float> loss = Loss.MSE(Y, Ygt, batch);
        loss.IsOutput = true;

        // Training loop
        float minLoss = float.MaxValue;
        DateTime lastShow = DateTime.Now;
        for (int i = 0; i < epochs; i++)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"LR: {lr:E2} | Epoch: {i} / {epochs}");
            // Forward and backward pass
            //loss.Forward();
            loss.Backward();

            // Build prediction data
            foreach (Dimdices dimdices in new Dimdexer(Y.Shape))
            {
                int j = dimdices[batch];
                float d = Y.Data[j];
                int val = Math.Abs(d - 1) < Math.Abs(d - 2) ? 1 : 2;
                preds[j] = new(v[j].X, [val]);
            }

            // Update weights
            cerebrin.Step(lr);
            // Reset gradients
            loss.ResetGradient();

            // Print loss and scatter plot
            Console.WriteLine($"Loss: {loss.Data[0]:E3} / {minLoss:E3}");
            if ((DateTime.Now - lastShow).TotalMilliseconds > 250)
            {
                lastShow = DateTime.Now;
                DataSet.Scatter(v, preds);
            }
            if (minLoss > loss.Data[0])
            {
                minLoss = loss.Data[0];
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
