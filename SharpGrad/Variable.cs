using SharpGrad.Activation;
using SharpGrad.Operators;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Variable<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public new TType Data { get => data; set => data = value; }

        public Variable(TType data, string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            base.data = data;
        }

        public override Expression GenerateForwardExpression()
        {
            return Expression.Field(Expression.Constant(this), nameof(data));
        }

        public static implicit operator Variable<TType>(TType d)
            => new(d, $"var{InstanceCount++}");
    }
}