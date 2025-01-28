using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ActivationReLU<TType> : Activation<TType>
        where TType : INumber<TType>, IComparable<TType>
    {
        public ActivationReLU(Value<TType> value)
            : base("relu", value)
        {
        }
        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Condition(
                Expression.LessThanOrEqual(Operand.GetAsOperand(variableExpressions), Expressions.Zero),
                Expressions.Zero,
                Operand.GetAsOperand(variableExpressions));

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
        }

    }
}