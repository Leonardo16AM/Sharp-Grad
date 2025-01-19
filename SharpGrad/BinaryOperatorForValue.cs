using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class BinaryOperatorForValue<TType> : OperatorForValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public ValueBase<TType> LeftChildren => Childrens[0];
        public ValueBase<TType> RightChildren => Childrens[1];

        public BinaryOperatorForValue(TType data, string name, ValueBase<TType> left, ValueBase<TType> right)
            : base(data, name, left, right)
        {
        }
    }


}