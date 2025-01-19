using SharpGrad.DifEngine;
using System;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class ReLUValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        public ReLUValue(Value<TType> value)
            : base(value.Data <= TType.Zero ? TType.Zero : value.Data, "relu", value)
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