using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class MulValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public MulValue(Value<TType> left, Value<TType> right)
            : base("*", left, right)
        {
        }

        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Multiply(LeftOperand.GenerateForwardExpression(), RightOperand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            LeftOperand!.Grad += Grad * RightOperand!.Data;
            RightOperand.Grad += Grad * LeftOperand.Data;
        }
    }
}