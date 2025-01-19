using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class SigmoidValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public SigmoidValue(Value<TType> value)
            : base(TType.One / (TType.One + TType.Exp(-value.Data)), "sigmoid", value)
        {
        }

        protected override void Backward()
        {
            var sigmoid = Data;
            Operand.Grad += Grad * sigmoid * (TType.One - sigmoid);
        }
    }
}
