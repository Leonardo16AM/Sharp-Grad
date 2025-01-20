using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class DivValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public DivValue(Value<TType> left, Value<TType> right)
            : base("/", left, right)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;

            expression = Expression.Divide(LeftOperand.GenerateForwardExpression(variableExpressions), RightOperand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        // TODO: Is this a good way to backpropagate division?
        protected override void Backward(TType accCount)
        {
            LeftOperand.Grad += Grad / RightOperand.Data / accCount;
            RightOperand.Grad += Grad * LeftOperand.Data / (RightOperand.Data * RightOperand.Data) / accCount;
        }
    }
}