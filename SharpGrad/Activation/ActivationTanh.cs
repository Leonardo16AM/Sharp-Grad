using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System;
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
        { }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Call(typeof(TType), nameof(TType.Tanh), null, Operand.GetAsOperand(variableExpressions));
        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression tanh = variableExpressions[this];
            Expression one = Expression.Constant(TType.One);
            Expression tanh2 = Expression.Multiply(tanh, tanh);
            Expression sub = Expression.Subtract(one, tanh2);
            Expression gr = Expression.Multiply(grad, sub);
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}
