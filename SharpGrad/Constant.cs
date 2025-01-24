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
        public Constant(TType data, string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            base.data = data;
            expression = Expression.Constant(base.data);
        }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, out Expression? operand)
        {
            operand = expression;
            return true;
        }

        public static implicit operator Constant<TType>(TType d)
            => new(d, $"c{InstanceCount++}");
    }
}