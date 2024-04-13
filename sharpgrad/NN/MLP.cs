using SharpGrad.DifEngine;

namespace SharpGrad.NN
{
    public class MLP
    {
        public List<Layer> Layers;
        public int Inputs;

        public MLP(int inputs, List<int> outputs)
        {
            Layers = new List<Layer>();
            Inputs = inputs;
            Layers.Add(new Layer(outputs[0], inputs, false));
            for (int i = 1; i < outputs.Count; i++)
            {
                Layers.Add(new Layer(outputs[i], outputs[i - 1], true));
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
    }
}