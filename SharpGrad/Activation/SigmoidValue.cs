using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public class SigmoidValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IExponentialFunctions<TType>
    {
        public SigmoidValue(Value<TType> value)
            : base(TType.One / (TType.One + TType.Exp(-value.Data)), "sigmoid", value)
        {
        }

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Divide(Expressions.One, Expression.Add(Expressions.One, Expression.Negate(Operand.GenerateForwardExpression())));
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            var sigmoid = Data;
            Operand.Grad += Grad * sigmoid * (TType.One - sigmoid);
        }
    }
}
