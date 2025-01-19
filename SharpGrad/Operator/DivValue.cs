using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class DivValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public DivValue(Value<TType> left, Value<TType> right)
            : base("/", left, right)
        {
        }

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Divide(LeftOperand.GenerateForwardExpression(), RightOperand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        // TODO: Is this a good way to backpropagate division?
        protected override void Backward()
        {
            LeftOperand.Grad += Grad / RightOperand.Data;
            RightOperand.Grad += Grad * LeftOperand.Data / (RightOperand.Data * RightOperand.Data);
        }
    }
}