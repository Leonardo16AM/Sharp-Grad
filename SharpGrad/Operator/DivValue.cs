using SharpGrad.DifEngine;
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


        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Divide(LeftOperand.GetAsOperand(variableExpressions), RightOperand.GetAsOperand(variableExpressions));

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