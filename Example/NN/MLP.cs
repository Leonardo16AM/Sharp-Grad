using SharpGrad;
using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public class MLP<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public List<Layer<TType>> Layers;
        public int Inputs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="count"></param>
        /// <param name="count"></param>
        public MLP(params int[] count)
        {
            if (count.Length < 2)
                throw new ArgumentException($"{nameof(count)} must have at least 2 elements. Got {count.Length}.");

            Layers = new List<Layer<TType>>(count.Length - 1);
            Inputs = count[0];
            Layers.Add(new Layer<TType>(count[1], Inputs, false));
            for (int i = 2; i < count.Length; i++)
            {
                Layers.Add(new Layer<TType>(count[i], count[i - 1], true));
            }
        }

        public List<Value<TType>> Forward(List<Value<TType>> X)
        {
            List<Value<TType>> Y;
            foreach (Layer<TType> l in Layers)
            {
                Y = l.Forward(X);
                X = Y;
            }
            return X;
        }

        public void Step(TType lr)
        {
            foreach (Layer<TType> l in Layers)
            {
                foreach (Neuron<TType> n in l.Neurons)
                {
                    foreach (Variable<TType> w in n.Weights)
                    {
                        // Console.WriteLine(w.data);
                        w.Data -= lr * w.Grad;
                    }
                    n.Biai.Data -= lr * n.Biai.Grad;
                }
            }
        }
    }
}