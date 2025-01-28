using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public abstract class Activation<TType>(string name, Value<TType> child) : UnariOpValue<TType>(name, child)
        where TType : INumber<TType>
    {
    }

    public class ActivationLeakyReLU<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        private readonly TType _alpha;

        public ActivationLeakyReLU(Value<TType> value, TType alpha)
            : base("leaky_relu", value)
        {
            _alpha = alpha;
        }

        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Condition(
                Expression.LessThanOrEqual(Operand.GetAsOperand(variableExpressions), Expressions.Zero),
                Expression.Multiply(Expression.Constant(_alpha), Operand.GetAsOperand(variableExpressions)),
                Operand.GetAsOperand(variableExpressions));

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
            else
                Operand.Grad += Grad * _alpha;
        }
    }
}
