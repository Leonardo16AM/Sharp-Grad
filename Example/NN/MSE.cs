using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static Variable<TType> MSE<TType>(List<Value<TType>> Y, List<Value<TType>> Y_hat)
            where TType : IBinaryFloatingPointIeee754<TType>
        {
            Variable<TType> loss = Variable<TType>.Zero;
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
