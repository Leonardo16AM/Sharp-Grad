using SharpGrad.DifEngine;

namespace SharpGrad.NN
{
    public class Layer
    {
        public List<Neuron> Neurons;
        public int NeuronsCount;
        public int Inputs;
        public bool ActFunc;

        public Layer(int neurons, int inputs, bool act_func)
        {
            Neurons = new List<Neuron>();
            for (int i = 0; i < neurons; i++)
            {
                Neurons.Add(new Neuron(inputs, act_func));
            }
        }

        public List<Value> Forward(List<Value> X)
        {
            List<Value> Y = new();
            foreach (Neuron n in Neurons)
            {
                Y.Add(n.Forward(X));
            }
            return Y;
        }
    }
}