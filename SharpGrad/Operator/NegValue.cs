using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public class NegValue<TType> : UnariOperation<TType>
        where TType : INumber<TType>
    {
        public NegValue(Value<TType> value)
            : base("-", value)
        { }

        internal override Expr GetForwardComputation(Expr operand)
            => -operand;

        protected override Expression ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList)
        {
            return Expression.Negate(gradientExpressions[this]);
        }
    }
}