using SharpGrad.DifEngine;
using System;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class PowValue<TType>(Value<TType> left, Value<TType> right)
        : BinaryOpValue<TType>(TType.Pow(left.Data, right.Data), "^", left, right)
        where TType : IBinaryFloatingPointIeee754<TType>, IPowerFunctions<TType>
    {
        protected override void Backward()
        {
            LeftOperand.Grad += Grad * RightOperand.Data * TType.Pow(LeftOperand.Data, RightOperand.Data - TType.One);
            RightOperand.Grad += Grad * TType.Pow(LeftOperand.Data, RightOperand.Data) * TType.Log(LeftOperand.Data);
        }
    }
}