using SharpGrad.DifEngine;
using SharpGrad.Operator;
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

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Call(typeof(TType), nameof(TType.Tanh), null, Operand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            var tanh = data;
            Operand!.Grad += Grad * (TType.One - tanh * tanh);
        }
    }
}
