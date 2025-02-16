using SharpGrad.Activation;
using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.NN
{
    public class Layer<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public static readonly Random Rand = new();

        public readonly Dimension[] Shape;

        public readonly Variable<TType> Weights;
        public readonly Variable<TType> Biai;
        public int NeuronsCount => Shape[0].Size;
        public int Inputs => Shape[1].Size;
        public bool ActFunc;

        public Layer(Dimension output, Dimension input, bool act_func)
        {
            Shape = [output, input];

            Weights = new Variable<TType>(Shape, "W");
            Dimdexer dimdexer = new(Shape);
            foreach (Dimdices dimdices in dimdexer)
            {
                Weights[dimdices] = TType.CreateSaturating(Rand.NextDouble());
            }

            Biai = new Variable<TType>([output], "B");
            dimdexer = new(Biai.Shape);
            foreach (Dimdices dimdices in dimdexer)
            {
                Biai[dimdices] = TType.CreateSaturating(Rand.NextDouble());
            }

            ActFunc = act_func;
        }

        public NariOperation<TType> Forward(Value<TType> X)
        {
            MulValue<TType> mul = X * Weights;
            SumValue<TType> sum = VMath.Sum(mul, Shape[1]);
            AddValue<TType> sumB = sum + Biai;
            return ActFunc ? sumB.ReLU() : sumB;
        }

        public void Step(TType lr)
        {
            Dimdexer dimdexer = new(Weights.Shape);
            foreach (Dimdices dimdices in dimdexer)
            {
                Weights[dimdices] -= lr * Weights.GetGradient(dimdices);
            }
        }
    }
}