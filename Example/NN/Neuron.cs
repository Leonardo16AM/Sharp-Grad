using SharpGrad.DifEngine;

namespace SharpGrad
{
    public class Neuron
    {
        public static readonly Random Rand = new();

        public readonly List<Value> Weights;
        public readonly Value Biai;
        public readonly int Inputs;
        public readonly bool ActFunc;

        public Neuron(int inputs, bool act_func)
        {
            Weights = new();
            Biai = new(Rand.NextDouble(), "B");
            Inputs = inputs;
            ActFunc = act_func;
            for (int i = 0; i < inputs; i++)
            {
                Weights.Add(new(Rand.NextDouble(), $"W{i}"));
            }
        }
        public Value Forward(List<Value> X)
        {
            Value sum = X[0] * Weights[0];
            for (int i = 1; i < Inputs; i++)
            {
                sum += X[i] * Weights[i];
            }
            sum += Biai;
            return ActFunc ? sum.ReLU() : sum;
        }
    }
}