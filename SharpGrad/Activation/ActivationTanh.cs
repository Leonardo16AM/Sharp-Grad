using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using SharpGrad.Operators;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationTanh<TType> : UnariOperation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public ActivationTanh(Value<TType> value)
            : base("tanh", value)
        { }

        internal override Expr GetForwardComputation(Expr operand)
            => Expression.Call(typeof(TType).GetMethod("Tanh")!, operand);

        protected override Expression ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expr grad = gradientExpressions[this];
            Expr tanh = variableExpressions[this];
            Expr one = Expression.Constant(TType.One);
            return grad * (one - (tanh * tanh));
        }
    }
}
