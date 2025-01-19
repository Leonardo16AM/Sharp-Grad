using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class BinaryOpValue<TType> : NariOpValue<TType>
        where TType : INumber<TType>
    {
        public Value<TType> LeftOperand => Childrens[0];
        public Value<TType> RightOperand => Childrens[1];

        public BinaryOpValue(TType data, string name, Value<TType> left, Value<TType> right)
            : base(data, name, left, right)
        {
        }
    }


}