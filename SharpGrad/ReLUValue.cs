using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class ReLUValue<TType> : Value<TType>
        where TType : IFloatingPointIeee754<TType>
    {
        public ReLUValue(Value<TType> value)
            : base((value.Data <= 0) ? 0 : value.Data, "relu", value)
        {
        }

        protected override void Backward()
        {
            if (Grad > 0)
                LeftChildren.Grad += Grad;
        }
    }
}