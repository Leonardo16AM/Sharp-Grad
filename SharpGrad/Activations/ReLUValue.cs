using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Activations
{
    public class ReLUValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public ReLUValue(Value<TType> value)
            : base(value.Data <= TType.Zero ? TType.Zero : value.Data, "relu", value)
        {
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                LeftChildren!.Grad += Grad;
        }
    }
}