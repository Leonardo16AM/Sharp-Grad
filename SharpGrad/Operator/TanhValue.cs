using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class TanhValue<TType> : UnariOpValue<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IHyperbolicFunctions<TType>
    {
        public TanhValue(Value<TType> value)
            : base(TType.Tanh(value.Data), "tanh", value)
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
            var tanh = Data;
            Operand!.Grad += Grad * (TType.One - tanh * tanh);
        }
    }
}
