
using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;

var v = DataSet.GetDataSet(400);
Console.WriteLine("Dataset:");
DataSet.Scatter(v);



MLP<float> cerebrin = new(2, 8, 1);

List<Value<float>> X =
[
    v[0].X[0],
    v[0].X[1]
];
List<Value<float>> Y = cerebrin.Forward(X);


Console.WriteLine(Y[0].Data);



int epochs = 1000;
float lr = 1e-9f;

float lastLoss = float.MaxValue;

for (int i = 0; i < epochs; i++)
{
    Console.WriteLine("Epoch: " + i);
    Value<float> loss = Value<float>.Zero;
    List<DataSet.Data> preds = [];

    for (int j = 0; j < v.Count; j++)
    {
        X =
        [
            v[j].X[0],
            v[j].X[1]
        ];
        Y = cerebrin.Forward(X);
        List<Value<float>> Ygt =
        [
            v[j].Y[0]
        ];
        var nl = loss + Loss.MSE(Y, Ygt);
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
        DataSet.Data nd = new(v[j].X, [val]);
        preds.Add(nd);
    }

    loss.Backpropagate();
    cerebrin.Step(lr);

    Console.WriteLine("Loss: " + loss.Data);
    DataSet.Scatter(preds);
    if (lastLoss > loss.Data)
    {
        lastLoss = loss.Data;
    }
    else
    {
        Console.WriteLine("Final loss: " + loss.Data);
        Console.WriteLine("Last epoch: " + i);
        Console.WriteLine("Loss is increasing. Stopping training...");
        break;
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