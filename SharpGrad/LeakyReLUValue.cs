using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class LeakyReLUValue<TType> : Value<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        private readonly TType _alpha;

        public LeakyReLUValue(Value<TType> value, TType alpha)
            : base((value.Data <= TType.Zero) ? alpha * value.Data : value.Data, "leaky_relu", value)
        {
            _alpha = alpha;
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                LeftChildren.Grad += Grad;
            else
                LeftChildren.Grad += Grad * _alpha;
        }
    }
}
