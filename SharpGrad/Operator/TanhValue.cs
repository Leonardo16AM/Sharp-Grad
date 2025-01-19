using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class TanhValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public TanhValue(Value<TType> value)
            : base(TType.Tanh(value.Data), "tanh", value)
        {
        }

        protected override void Backward()
        {
            var tanh = Data;
            Operand!.Grad += Grad * (TType.One - tanh * tanh);
        }
    }
}
