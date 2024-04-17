using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;

public class MLP
{
    public List<Layer> Layers;
    public int Inputs;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="count"></param>
    /// <param name="count"></param>
    public MLP(params int[] count)
    {
        if(count.Length < 2)
            throw new ArgumentException($"{nameof(count)} must have at least 2 elements. Got {count.Length}.");

        Layers = new List<Layer>(count.Length - 1);
        Inputs = count[0];
        Layers.Add(new Layer(count[1], Inputs, false));
        for (int i = 2; i < count.Length; i++)
        {
            Layers.Add(new Layer(count[i], count[i - 1], true));
        }
    }

    public List<Value> Forward(List<Value> X)
    {
        List<Value> Y;
        foreach (Layer l in Layers)
        {
            Y = l.Forward(X);
            X = Y;
        }
        return X;
    }

    public void Step(double lr)
    {
        foreach (Layer l in Layers)
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
    }
}