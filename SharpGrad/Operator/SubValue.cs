using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class SubValue<TType> : BinaryOpValue<TType>
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

        public SubValue(Value<TType> left, Value<TType> right)
            : base(GetShape(left, right), "-", left, right)
        { }

        protected override Expression GetForwardComputation(Expression left, Expression right)
            => Expression.Subtract(left, right);

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, grad);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = Expression.Negate(gradientExpressions[this]);
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, grad);
        }
    }
}