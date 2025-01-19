using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static Value<TType> MSE<TType>(List<Value<TType>> Y, List<Value<TType>> Y_hat)
            where TType : IBinaryFloatingPointIeee754<TType>
        {
            Value<TType> loss = Value<TType>.Zero;
            for (int i = 0; i < Y.Count; i++)
            {
                var yDiff = Y[i] - Y_hat[i];
                var nl = loss + (yDiff * yDiff);
                loss = nl;
            }
            return loss;
        }
    }
}
