using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class DivValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public DivValue(ValueBase<TType> left, ValueBase<TType> right)
            : base(left.Data / right.Data, "/", left, right)
        {
        }

        // TODO: Is this a good way to backpropagate division?
        protected override void Backward()
        {
            LeftOperand.Grad += Grad / RightOperand.Data;
            RightOperand.Grad += Grad * LeftOperand.Data / (RightOperand.Data * RightOperand.Data);
        }
    }
}