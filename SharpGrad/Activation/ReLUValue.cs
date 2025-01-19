using SharpGrad.DifEngine;
using System;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ReLUValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        public ReLUValue(Value<TType> value)
            : base(value.Data <= TType.Zero ? TType.Zero : value.Data, "relu", value)
        {
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
        }
    }
}