using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System;
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
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left - right;

        protected override Expression ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            return gradientExpressions[this];
        }

        protected override Expression ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            return Expression.Negate(gradientExpressions[this]);
        }
    }
}