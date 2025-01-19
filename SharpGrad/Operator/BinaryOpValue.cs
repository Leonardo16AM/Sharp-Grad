using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class BinaryOpValue<TType> : NariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public ValueBase<TType> LeftOperand => Childrens[0];
        public ValueBase<TType> RightOperand => Childrens[1];

        public BinaryOpValue(TType data, string name, ValueBase<TType> left, ValueBase<TType> right)
            : base(data, name, left, right)
        {
        }
    }


}