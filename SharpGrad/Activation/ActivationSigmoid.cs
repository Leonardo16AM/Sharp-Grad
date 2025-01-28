using SharpGrad.DifEngine;
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
        {
        }

        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Divide(Expressions.One, Expression.Add(Expressions.One, Expression.Call(typeof(TType).GetMethod("Exp")!, Expression.Negate(Operand.GetAsOperand(variableExpressions)))));

        internal override void Backward(TType accCount)
        {
            var sigmoid = data;
            Operand.Grad += Grad * sigmoid * (TType.One - sigmoid) / accCount;
        }
    }
}
