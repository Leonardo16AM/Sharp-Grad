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
        public static int GetShape(Value<TType> value)
            => value.Shape;

        public ActivationReLU(Value<TType> value)
            : base(GetShape(value), "relu", value)
        { }

        internal override Expr GetForwardComputation(Expr operand)
            => Expr.Condition(
                Expr.LessThanOrEqual(operand, ExpressionZero),
                ExpressionZero,
                operand);

        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression relu = variableExpressions[this];
            Expression gr = Expression.Condition(
                Expression.LessThanOrEqual(relu, Expression.Constant(TType.Zero)),
                Expression.Constant(TType.Zero),
                grad);
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}