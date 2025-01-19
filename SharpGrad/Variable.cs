using SharpGrad.Activation;
using SharpGrad.Operators;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Variable<TType>(TType data, string name, params Value<TType>[] childs)
        : Value<TType>(data, name, childs)
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public override Expression GenerateForwardExpression()
        {
            return Expression.Field(Expression.Constant(this), nameof(Data));
        }

        public static implicit operator Variable<TType>(TType d)
            => new Variable<TType>(d, $"v{InstanceCount++}");
    }
}