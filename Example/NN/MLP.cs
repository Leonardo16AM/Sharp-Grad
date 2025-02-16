using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.NN
{
    public class MLP<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public Dimension[] Shape;
        public Layer<TType>[] Layers;
        public int Inputs => Shape[0].Size;
        public int Outputs => Shape[^1].Size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="shape"></param>
        /// <param name="shape"></param>
        public MLP(params Dimension[] shape)
        {
            if (shape.Length < 2)
                throw new ArgumentException($"{nameof(shape)} must have at least 2 dimension. Got {shape.Length}.");

            Shape = shape;
            Layers = new Layer<TType>[shape.Length - 1];
            Layers[0] = new Layer<TType>(shape[1], shape[0], false);
            for (int i = 2; i < shape.Length; i++)
            {
                Layers[i - 1] = new Layer<TType>(shape[i], shape[i - 1], true);
            }
        }

        public Value<TType> Forward(Value<TType> X)
        {
            Value<TType>? output = X;
            for(int i = 0; i < Layers.Length; i++)
            {
                output = Layers[i].Forward(output);
            }
            output!.IsOutput = true;
            return output;
        }

        public void Step(TType lr)
        {
            foreach (Layer<TType> l in Layers)
            {
                l.Step(lr);
            }
        }
    }
}