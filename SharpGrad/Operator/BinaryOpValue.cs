using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class BinaryOpValue<TType>(string name, Value<TType> left, Value<TType> right) :
        NariOpValue<TType>(name, left, right)
        where TType : INumber<TType>
    {
        public Value<TType> LeftOperand => Childrens[0];
        public Value<TType> RightOperand => Childrens[1];
    }
}