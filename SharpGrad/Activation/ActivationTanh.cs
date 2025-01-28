using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationTanh<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public ActivationTanh(Value<TType> value)
            : base("tanh", value)
        {
        }

        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Call(typeof(TType), nameof(TType.Tanh), null, Operand.GetAsOperand(variableExpressions));

        protected override void Backward()
        {
            var tanh = data;
            Operand!.Grad += Grad * (TType.One - tanh * tanh);
        }

    }
}
