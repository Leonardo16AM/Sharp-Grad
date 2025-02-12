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

        protected override Expression ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'l' in 'l / r' is 'g / r'
            Expr thisGrad = gradientExpressions[this];
            Expr rightVal = variableExpressions[RightOperand];
            return thisGrad / rightVal;
        }

        protected override Expression ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'r' in 'l / r' is 'g * -l / r^2'
            Expr thisGrad = gradientExpressions[this];
            Expr leftVal = variableExpressions[LeftOperand];
            Expr rightVal = variableExpressions[RightOperand];
            return thisGrad * (-leftVal / (rightVal * rightVal));
        }
    }
}