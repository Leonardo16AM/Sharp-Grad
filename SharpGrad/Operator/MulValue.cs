using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class MulValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public MulValue(Value<TType> left, Value<TType> right)
            : base(left.Data * right.Data, "*", left, right)
        {
        }

        protected override void Backward()
        {
            LeftOperand!.Grad += Grad * RightOperand!.Data;
            RightOperand.Grad += Grad * LeftOperand.Data;
        }
    }
}