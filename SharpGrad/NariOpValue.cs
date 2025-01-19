using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad
{
    public abstract class NariOpValue<TType> : Value<TType>
    where TType : INumber<TType>
    {
        public NariOpValue(TType data, string name, params Value<TType>[] childs)
            : base(data, name, childs)
        {
            if (childs.Length < 1)
                throw new System.ArgumentException($"Operator {name} must have at least one child.");
        }
    }
}