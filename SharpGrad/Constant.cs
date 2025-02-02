using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad
{
    public class Constant<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        private readonly Expression expression;
        internal Expression Expression => expression;

        public Constant(TType[] data, string name, params Value<TType>[] childs)
            : base(data.Length, name, childs)
        {
            base.data = data;
            Expression field = Expression.Field(Expression.Constant(this), nameof(data));
            // TODO: !!! DON'T USE Expression.Constant(0) !!!
            Expression arrayAccess = Expression.ArrayAccess(field, Expression.Constant(0));
            expression = arrayAccess;
        }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, out Expression? operand)
        {
            operand = expression;
            return true;
        }

        public static implicit operator Constant<TType>(TType d)
            => new([d], $"c{InstanceCount++}");
    }
}