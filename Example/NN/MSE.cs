using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static Value<TType> MSE<TType>(IReadOnlyList<Value<TType>> Y, IReadOnlyList<Value<TType>> Y_hat)
            where TType : INumber<TType>
        {
            Value<TType> loss = Value<TType>.Zero;
            for (int i = 0; i < Y.Count; i++)
            {
                var yDiff = Y[i] - Y_hat[i];
                loss += yDiff * yDiff;
            }
            return loss;
        }
    }
}
