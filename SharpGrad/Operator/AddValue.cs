using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class AddValue<TType> : BinaryOperation<TType>
        where TType : INumber<TType>
    {
        public AddValue(Value<TType> left, Value<TType> right)
            : base("+", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => left + right;

        protected override Expression ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'l' in 'l + r' is 1 * 'g'
            return gradientExpressions[this];
        }

        protected override Expression ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            // Gradient of 'r' in 'l + r' is 1 * 'g'
            return gradientExpressions[this];
        }
    }
}