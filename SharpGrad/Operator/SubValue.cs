using SharpGrad.DifEngine;
using System.Collections.Generic;
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

        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Subtract(LeftOperand.GetAsOperand(variableExpressions), RightOperand.GetAsOperand(variableExpressions));

        protected override void Backward()
        {
            LeftOperand.Grad += Grad;
            RightOperand.Grad -= Grad;
        }
    }
}