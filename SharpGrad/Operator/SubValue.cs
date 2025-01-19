using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class SubValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public SubValue(Value<TType> left, Value<TType> right)
            : base("-", left, right)
        {
        }

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Subtract(LeftOperand.GenerateForwardExpression(), RightOperand.GenerateForwardExpression());
            Expression assign = Expression.Assign(DataExpression, expression);
            return assign;
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad -= Grad;
        }
    }
}