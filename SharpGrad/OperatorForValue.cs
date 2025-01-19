using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class OperatorForValue<TType> : Value<TType>
    where TType : IBinaryFloatingPointIeee754<TType>
    {
        public OperatorForValue(TType data, string name, params ValueBase<TType>[] childs)
            : base(data, name, childs)
        {
            if (childs.Length < 1)
                throw new System.ArgumentException($"Operator {name} must have at least one child.");
        }
    }


}