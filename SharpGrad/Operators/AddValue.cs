using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class AddValue<TType> : BinaryOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public AddValue(ValueBase<TType> left, ValueBase<TType> right)
            : base(left.Data + right.Data, "+", left, right)
        {
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad += Grad;
        }
    }


}