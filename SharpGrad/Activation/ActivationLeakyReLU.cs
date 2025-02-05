using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.Activation
{
    public class ActivationLeakyReLU<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        private readonly TType _alpha;
        private readonly Expression alphaExpression;

        public ActivationLeakyReLU(Value<TType> value, TType alpha)
            : base("leaky_relu", value)
        {
            _alpha = alpha;
            alphaExpression = Expression.Constant(alpha);
        }

        internal override Expr GetForwardComputation(Expr operand)
            => Expr.Condition(
                Expr.LessThanOrEqual(operand, ExpressionZero),
                alphaExpression * operand,
                operand);

        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression relu = variableExpressions[this];
            Expression gr = Expression.Condition(
                Expression.LessThanOrEqual(relu, Expression.Constant(TType.Zero)),
                Expression.Multiply(Expression.Constant(_alpha), grad),
                grad);
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}
