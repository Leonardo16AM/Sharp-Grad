using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Activations
{
    public class LeakyReLUValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        private readonly TType _alpha;

        public LeakyReLUValue(Value<TType> value, TType alpha)
            : base(value.Data <= TType.Zero ? alpha * value.Data : value.Data, "leaky_relu", value)
        {
            _alpha = alpha;
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand!.Grad += Grad;
            else
                Operand!.Grad += Grad * _alpha;
        }
    }
}
