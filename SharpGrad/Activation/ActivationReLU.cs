using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationReLU<TType> : Activation<TType>
        where TType : INumber<TType>, IComparable<TType>
    {
        public ActivationReLU(Value<TType> value)
            : base("relu", value)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;

            expression = Expression.Condition(
                Expression.LessThanOrEqual(Operand.GenerateForwardExpression(variableExpressions), Expressions.Zero),
                Expressions.Zero,
                Operand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
        }
    }
}