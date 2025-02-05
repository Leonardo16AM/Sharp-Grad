using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class AddValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public AddValue(Value<TType> left, Value<TType> right)
            : base("+", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left + right;

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gradientExpressions[this]);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, gradientExpressions[this]);
        }
    }
}