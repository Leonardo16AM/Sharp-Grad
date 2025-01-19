using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class UnariOperatorForValue<TType> : OperatorForValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public ValueBase<TType> LeftChildren => Childrens[0];

        public UnariOperatorForValue(TType data, string name, ValueBase<TType> child)
            : base(data, name, child)
        {
        }
    }


}