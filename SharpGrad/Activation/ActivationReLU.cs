﻿using SharpGrad.DifEngine;
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

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Condition(
                Expression.LessThanOrEqual(Operand.GetAsOperand(variableExpressions), ExpressionZero),
                ExpressionZero,
                Operand.GetAsOperand(variableExpressions));

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