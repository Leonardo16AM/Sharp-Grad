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
        protected override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Call(typeof(TType).GetMethod("Pow")!,
                LeftOperand.GetAsOperand(variableExpressions),
                RightOperand.GetAsOperand(variableExpressions));


        protected override void Backward()
        {
            LeftOperand.Grad += Grad * RightOperand.Data * TType.Pow(LeftOperand.Data, RightOperand.Data - TType.One);
            RightOperand.Grad += Grad * TType.Pow(LeftOperand.Data, RightOperand.Data) * TType.Log(LeftOperand.Data);
        }

    }
}