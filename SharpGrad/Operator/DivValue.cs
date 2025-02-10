using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class DivValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public DivValue(Value<TType> left, Value<TType> right)
            : base("/", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left / right;

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'l' in 'l / r' is 'g / r'
            Expr grad = gradientExpressions[this];
            Expr right = variableExpressions[RightOperand];
            Expr gr = grad / right;
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'r' in 'l / r' is '-l * g / r^2'
            Expr thisGrad = gradientExpressions[this];
            Expression leftVal = variableExpressions[LeftOperand];
            Expr rightVal = variableExpressions[RightOperand];
            Expr result = -(leftVal * thisGrad) / rightVal * rightVal;
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, result);
        }
    }
}