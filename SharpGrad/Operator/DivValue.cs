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
        public static int GetShape(Value<TType> left, Value<TType> right)
        {
            if (left.Shape != right.Shape)
            {
                throw new ArgumentException("Shapes of left and right operands must be equal.");
            }
            return left.Shape;
        }

        public DivValue(Value<TType> left, Value<TType> right)
            : base(GetShape(left, right), "/", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left / right;

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression right = variableExpressions[RightOperand];
            Expression gr = Expression.Divide(grad, right);
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression thisGrad = gradientExpressions[this];
            Expression leftVal = variableExpressions[LeftOperand];
            Expression rightVal = variableExpressions[RightOperand];
            Expression lg = Expression.Multiply(leftVal, thisGrad);
            Expression rr = Expression.Multiply(rightVal, rightVal);
            Expression lgrr = Expression.Divide(lg, rr);
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, lgrr);
        }
    }
}