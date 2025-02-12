using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class MulValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public MulValue(Value<TType> left, Value<TType> right)
            : base("*", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left * right;

        protected override Expression ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'l' in 'l * r' is 'g * r'
            Expr grad = gradientExpressions[this];
            Expr right = variableExpressions[RightOperand];
            return grad * right;
        }

        protected override Expression ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'r' in 'l * r' is 'g * l'
            Expr grad = gradientExpressions[this];
            Expr left = variableExpressions[LeftOperand];
            return left * grad;
        }
    }
}