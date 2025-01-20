using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class AddValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public AddValue(Value<TType> left, Value<TType> right)
            : base("+", left, right)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;
            Expression compute = Expression.Add(LeftOperand.GenerateForwardExpression(variableExpressions), RightOperand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, compute);
        }

        protected override void Backward(TType accCount)
        {
            LeftOperand.Grad += Grad / accCount;
            RightOperand.Grad += Grad / accCount;
        }
    }
}