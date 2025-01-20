using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class PowValue<TType>(Value<TType> left, Value<TType> right) :
        BinaryOpValue<TType>("^", left, right)
        where TType : INumber<TType>, IPowerFunctions<TType>, ILogarithmicFunctions<TType>
    {
        public override Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            if (variableExpressions.TryGetValue(this, out Expression? expression))
                return expression;

            expression = Expression.Call(
                typeof(TType).GetMethod("Pow")!,
                LeftOperand.GenerateForwardExpression(variableExpressions),
                RightOperand.GenerateForwardExpression(variableExpressions));
            variableExpressions[this] = DataExpression;
            return Expression.Assign(DataExpression, expression);
        }

        protected override void Backward(TType accCount)
        {
            LeftOperand.Grad += Grad * RightOperand.Data * TType.Pow(LeftOperand.Data, RightOperand.Data - TType.One) / accCount;
            RightOperand.Grad += Grad * TType.Pow(LeftOperand.Data, RightOperand.Data) * TType.Log(LeftOperand.Data) / accCount;
        }
    }
}