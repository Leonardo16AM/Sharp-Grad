using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class SubValue<TType> : BinaryOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public SubValue(Value<TType> left, Value<TType> right)
            : base(left.Data - right.Data, "-", left, right)
        {
        }

        protected override void Backward()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad -= Grad;
        }
    }
}