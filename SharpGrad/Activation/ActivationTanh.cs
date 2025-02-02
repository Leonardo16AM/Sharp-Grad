using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using SharpGrad.Operator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationTanh<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public static int GetShape(Value<TType> value)
            => value.Shape;

        public ActivationTanh(Value<TType> value)
            : base(GetShape(value), "tanh", value)
        { }

        internal override Expr GetForwardComputation(Expr operand)
            => Expression.Call(typeof(TType).GetMethod("Tanh")!, operand);

        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression tanh = variableExpressions[this];
            Expression one = Expression.Constant(TType.One);
            Expression tanh2 = Expression.Multiply(tanh, tanh);
            Expression sub = Expression.Subtract(one, tanh2);
            Expression gr = Expression.Multiply(grad, sub);
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}
