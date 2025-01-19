using SharpGrad.DifEngine;
using System;
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

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Condition(
                Expression.LessThanOrEqual(Operand.GenerateForwardExpression(), Expressions.Zero),
                Expressions.Zero,
                Operand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
        }
    }
}