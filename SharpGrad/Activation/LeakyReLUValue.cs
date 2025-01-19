using SharpGrad.DifEngine;
using System;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class LeakyReLUValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        private readonly TType _alpha;

        public LeakyReLUValue(Value<TType> value, TType alpha)
            : base(value.Data <= TType.Zero ? alpha * value.Data : value.Data, "leaky_relu", value)
        {
            _alpha = alpha;
        }

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Condition(
                Expression.LessThanOrEqual(Operand.GenerateForwardExpression(), Expressions.Zero),
                Expression.Multiply(Expression.Constant(_alpha), Operand.GenerateForwardExpression()),
                Operand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            if (Grad > TType.Zero)
                Operand.Grad += Grad;
            else
                Operand.Grad += Grad * _alpha;
        }
    }
}
