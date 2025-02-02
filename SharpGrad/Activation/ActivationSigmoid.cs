using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationSigmoid<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IExponentialFunctions<TType>
    {
        public static int GetShape(Value<TType> value)
            => value.Shape;
        public ActivationSigmoid(Value<TType> value)
            : base(GetShape(value), "sigmoid", value)
        { }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Divide(ExpressionOne, Expression.Add(ExpressionOne, Expression.Call(typeof(TType).GetMethod("Exp")!, Expression.Negate(Operand.GetAsOperand(variableExpressions)))));

        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression sigmoid = variableExpressions[this];
            Expression sub = Expression.Subtract(ExpressionOne, sigmoid);
            Expression gr = Expression.Multiply(grad, Expression.Multiply(sigmoid, sub));
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}
