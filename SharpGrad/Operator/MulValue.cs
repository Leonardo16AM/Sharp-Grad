using SharpGrad.DifEngine;
using System.Collections.Generic;
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

        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Multiply(LeftOperand.GetAsOperand(variableExpressions), RightOperand.GetAsOperand(variableExpressions));

        internal override void Backward(TType accCount)
        {
            LeftOperand!.Grad += Grad * RightOperand!.Data / accCount;
            RightOperand.Grad += Grad * LeftOperand.Data / accCount;
        }

    }
}