﻿using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public class NegValue<TType> : UnariOpValue<TType>
        where TType : INumber<TType>
    {
        public NegValue(Value<TType> value)
            : base("-", value)
        { }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Negate(Operand.GetAsOperand(variableExpressions));

        protected override void ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            AssignGradientExpession(gradientExpressions, expressionList, Operand, Expression.Negate(grad));
        }
    }
}