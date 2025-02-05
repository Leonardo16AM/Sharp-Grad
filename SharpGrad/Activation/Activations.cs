using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Activation
{
    public static class Activations
    {
        public static ActivationReLU<TType> ReLU<TType>(this Value<TType> value)
            where TType : INumber<TType>
            => new(value);

        public static ActivationLeakyReLU<TType> LeakyReLU<TType>(this Value<TType> value, TType alpha)
            where TType : INumber<TType>
            => new(value, alpha);

        public static ActivationTanh<TType> Tanh<TType>(this Value<TType> value)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value);

        public static ActivationSigmoid<TType> Sigmoid<TType>(this Value<TType> value)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value);
    }
}