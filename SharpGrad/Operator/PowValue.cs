using SharpGrad.DifEngine;
using System;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class PowValue<TType>(Value<TType> left, Value<TType> right)
        : BinaryOpValue<TType>(TType.Pow(left.Data, right.Data), "^", left, right)
        where TType : INumber<TType>, IPowerFunctions<TType>, ILogarithmicFunctions<TType>
    {
        public override Expression GenerateForwardExpression()
        {
            Expression expression = Expression.Power(LeftOperand.GenerateForwardExpression(), RightOperand.GenerateForwardExpression());
            Expression assignExpression = Expression.Assign(DataExpression, expression);
            return assignExpression;
        }

        protected override void Backward()
        {
            LeftOperand.Grad += Grad * RightOperand.Data * TType.Pow(LeftOperand.Data, RightOperand.Data - TType.One);
            RightOperand.Grad += Grad * TType.Pow(LeftOperand.Data, RightOperand.Data) * TType.Log(LeftOperand.Data);
        }
    }
}