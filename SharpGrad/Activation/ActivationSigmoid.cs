using SharpGrad.DifEngine;
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

        public override Expression GenerateForwardExpression()
        {
            // 1 / (1 + exp(-x))
            var minusX = Expression.Negate(Operand.GenerateForwardExpression());
            var expMinusX = Expression.Call(typeof(TType).GetMethod("Exp")!, minusX);

            Expression expression = Expression.Divide(Expressions.One, Expression.Add(Expressions.One, expMinusX));
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            var sigmoid = data;
            Operand.Grad += Grad * sigmoid * (TType.One - sigmoid);
        }
    }
}
