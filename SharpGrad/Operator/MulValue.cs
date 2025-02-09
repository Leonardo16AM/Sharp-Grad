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

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'l' in 'l * r' is 'g * r'
            Expression grad = gradientExpressions[this];
            Expression right = variableExpressions[RightOperand];
            Expression gr = Expression.Multiply(grad, right);
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'r' in 'l * r' is 'g * l'
            Expression grad = gradientExpressions[this];
            Expression left = variableExpressions[LeftOperand];
            Expression lg = Expression.Multiply(left, grad);
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, lg);
        }
    }
}