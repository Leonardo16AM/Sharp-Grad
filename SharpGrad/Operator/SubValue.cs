using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class SubValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public SubValue(Value<TType> left, Value<TType> right)
            : base("-", left, right)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;

            expression = Expression.Subtract(LeftOperand.GenerateForwardExpression(variableExpressions), RightOperand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad -= Grad;
        }
    }
}