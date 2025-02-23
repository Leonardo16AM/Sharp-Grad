using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public static class VMath
    {
        public static PowValue<TType> Pow<TType>(this Value<TType> @this, Value<TType> operand)
            where TType : IBinaryFloatingPointIeee754<TType>, IPowerFunctions<TType>
            => new(@this, operand);

        public static SumValue<TType> Sum<TType>(this Value<TType> @this, params Dimension[] toReduce)
            where TType : INumber<TType>
        {
            if (toReduce.Length == 0)
            {
                throw new ArgumentException("The dimensions to reduce must be specified");
            }
            else
            {
                List<Dimension> newShape = [.. @this.Shape];
                foreach (Dimension dim in toReduce)
                {
                    if (!newShape.Remove(dim))
                    {
                        throw new ArgumentException($"The dimension {dim} is not present in the shape {@this.Shape}");
                    }
                }
                return new SumValue<TType>([.. newShape], "∑", @this);
            }
        }
        public static SumValue<TType> Sum<TType>(this Value<TType> @this, Dimension toReduce)
            where TType : IBinaryFloatingPointIeee754<TType>
            => Sum(@this, [toReduce]);

    }
}