using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class MulValue<TType> : Value<TType>
        where TType : IFloatingPointIeee754<TType>
    {
        public MulValue(Value<TType> left, Value<TType> right)
            : base(left.Data * right.Data, "*", left, right)
        {
        }

        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data;
            RightChildren.Grad += Grad * LeftChildren.Data;
        }
    }
}