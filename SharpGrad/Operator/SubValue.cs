using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class SubValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public SubValue(ValueBase<TType> left, ValueBase<TType> right)
            : base(left.Data - right.Data, "-", left, right)
        {
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad -= Grad;
        }
    }
}