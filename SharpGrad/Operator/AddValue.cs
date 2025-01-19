using SharpGrad.DifEngine;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class AddValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public AddValue(Value<TType> left, Value<TType> right)
            : base("+", left, right)
        {
        }

        public override Expression GenerateForwardExpression()
        {
            Expression compute = Expression.Add(LeftOperand.GenerateForwardExpression(), RightOperand.GenerateForwardExpression());
            Expression assign = Expression.Assign(DataExpression, compute);
            return assign;
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad += Grad;
        }
    }
}