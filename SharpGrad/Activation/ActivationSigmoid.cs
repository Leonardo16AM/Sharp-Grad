using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationSigmoid<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IExponentialFunctions<TType>
    {
        public ActivationSigmoid(Value<TType> value)
            : base("sigmoid", value)
        { }

        internal override Expr GetForwardComputation(Expr operand)
        {
            Expr negated = -operand;
            Expr exp = Expr.Call(typeof(TType).GetMethod("Exp")!, negated);
            Expr add = ExpressionOne + exp;
            return Expression.Divide(ExpressionOne, add);
        }

        protected override Expression ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expr grad = gradientExpressions[this];
            Expr sigmoid = variableExpressions[this];
            Expr sub = ExpressionOne - sigmoid;
            return grad * sigmoid * sub;
        }
    }
}
