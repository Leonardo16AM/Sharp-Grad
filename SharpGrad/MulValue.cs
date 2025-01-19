using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class MulValue<TType> : BinaryOperatorForValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public MulValue(Value<TType> left, Value<TType> right)
            : base(left.Data * right.Data, "*", left, right)
        {
        }

        protected override void Backward()
        {
            LeftChildren!.Grad += Grad * RightChildren!.Data;
            RightChildren.Grad += Grad * LeftChildren.Data;
        }
    }
}