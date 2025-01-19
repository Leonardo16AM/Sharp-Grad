using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Activation
{
    public abstract class UnariOpValue<TType> : NariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public ValueBase<TType> Operand => Childrens[0];

        public UnariOpValue(TType data, string name, ValueBase<TType> child)
            : base(data, name, child)
        {
        }
    }


}