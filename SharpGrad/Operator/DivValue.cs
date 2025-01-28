using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class DivValue<TType> : BinaryOpValue<TType>
        where TType : INumber<TType>
    {
        public DivValue(Value<TType> left, Value<TType> right)
            : base("/", left, right)
        { }


        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Divide(LeftOperand.GetAsOperand(variableExpressions), RightOperand.GetAsOperand(variableExpressions));

        // TODO: Is this a good way to backpropagate division?
        protected override void Backward()
        {
            LeftOperand.Grad += Grad / RightOperand.Data;
            RightOperand.Grad += Grad * LeftOperand.Data / (RightOperand.Data * RightOperand.Data);
        }

    }
}