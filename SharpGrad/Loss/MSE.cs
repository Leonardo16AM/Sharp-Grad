using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static NariOperation<TType> MSE<TType>(Value<TType> Y, Value<TType> Y_hat, Dimension batch)
            where TType : INumber<TType>
        {
            NariOperation<TType> loss = Y - Y_hat;
            loss *= loss;
            loss = VMath.Sum(loss, batch);
            loss /= TType.CreateTruncating(batch.Size);
            return loss;
        }
    }
}
