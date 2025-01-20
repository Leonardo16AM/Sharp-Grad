using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationTanh<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public ActivationTanh(Value<TType> value)
            : base("tanh", value)
        {
        }

        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if(variableExpressions.TryGetValue(Operand, out Expression? expression))
                return expression;

            expression = Expression.Call(typeof(TType), nameof(TType.Tanh), null, Operand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        protected override void Backward(TType accCount)
        {
            var tanh = data;
            Operand!.Grad += Grad * (TType.One - tanh * tanh) / accCount;
        }
    }
}
