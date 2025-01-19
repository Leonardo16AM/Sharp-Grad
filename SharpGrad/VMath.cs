using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public static class VMath
    {
        public static PowValue<TType> Pow<TType>(this ValueBase<TType> left, Value<TType> right)
            where TType : IBinaryFloatingPointIeee754<TType>, IPowerFunctions<TType>
            => new(left, right);
    }
}