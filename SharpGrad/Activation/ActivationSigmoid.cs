using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationSigmoid<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IExponentialFunctions<TType>
    {
        public ActivationSigmoid(Value<TType> value)
            : base("sigmoid", value)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;

            // 1 / (1 + exp(-x))
            var minusX = Expression.Negate(Operand.GenerateForwardExpression(variableExpressions));
            var expMinusX = Expression.Call(typeof(TType).GetMethod("Exp")!, minusX);

            expression = Expression.Divide(Expressions.One, Expression.Add(Expressions.One, expMinusX));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        protected override void Backward()
        {
            var sigmoid = data;
            Operand.Grad += Grad * sigmoid * (TType.One - sigmoid);
        }
    }
}
