using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
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
        { }

        internal override Expr GetForwardComputation(Expr operand)
            => Expr.Condition(
                Expr.LessThanOrEqual(operand, ExpressionZero),
                ExpressionZero,
                operand);

        protected override Expression ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expr grad = gradientExpressions[this];
            Expr relu = variableExpressions[this];
            return Expression.Condition(
                Expression.LessThanOrEqual(relu, Expression.Constant(TType.Zero)),
                Expression.Constant(TType.Zero),
                grad);
        }
    }
}