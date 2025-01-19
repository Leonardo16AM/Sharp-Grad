using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.Activation
{
    public static class Activations
    {
        public static ReLUValue<TType> ReLU<TType>(this ValueBase<TType> value)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value);

        public static LeakyReLUValue<TType> LeakyReLU<TType>(this ValueBase<TType> value, TType alpha)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value, alpha);

        public static TanhValue<TType> Tanh<TType>(this ValueBase<TType> value)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value);

        public static SigmoidValue<TType> Sigmoid<TType>(this ValueBase<TType> value)
            where TType : IBinaryFloatingPointIeee754<TType>
            => new(value);
    }
}