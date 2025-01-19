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
            LeftChildren.Grad += Grad * RightChildren.Data * TType.Pow(LeftChildren.Data, RightChildren.Data - TType.One);
            RightChildren.Grad += Grad * TType.Pow(LeftChildren.Data, RightChildren.Data) * TType.Log(LeftChildren.Data);
        }
    }
}