using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Variable<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        private readonly Expression expression;

        public new TType Data { get => data; set => data = value; }

        public Variable(TType data, string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            base.data = data;
            expression = Expression.Field(Expression.Constant(this), nameof(data));
        }

        public Variable(string name, params Value<TType>[] childs) :
            this(TType.Zero, name, childs)
        { }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
            => expression;

        public static implicit operator Variable<TType>(TType d)
            => new(d, $"var{InstanceCount++}");
    }
}