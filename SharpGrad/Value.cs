using SharpGrad.Activation;
using SharpGrad.Operators;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Value<TType>(TType data, string name, params ValueBase<TType>[] childs)
        : ValueBase<TType>(data, name, childs)
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public static readonly Value<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Value<TType> Zero = new(TType.Zero, "0");

        public delegate void BackwardPass();

        public override Expression GenerateExpression()
        {
            return Expression.Field(Expression.Constant(this), nameof(Data));
        }

        public static implicit operator Value<TType>(TType d)
            => new(d, $"value_{++InstanceCount}");
        public static explicit operator TType(Value<TType> v)
            => v.Data;
    }
}