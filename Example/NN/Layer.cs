using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public class Layer<TType>
        where TType : IFloatingPoint<TType>
    {
        public List<Neuron<TType>> Neurons;
        public int NeuronsCount;
        public int Inputs;
        public bool ActFunc;

        public Layer(int neurons, int inputs, bool act_func)
        {
            Neurons = [];
            for (int i = 0; i < neurons; i++)
            {
                Neurons.Add(new Neuron<TType>(inputs, act_func));
            }
        }

        public List<Value<TType>> Forward(List<Value<TType>> X)
        {
            List<Value<TType>> Y = [];
            foreach (Neuron<TType> n in Neurons)
            {
                Y.Add(n.Forward(X));
            }
            return Y;
        }
    }
}