using System;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class PowValue<TType>(Value<TType> left, Value<TType> right) : Value<TType>(Math.Pow(left.Data, right.Data), "^", left, right)
            where TType : IFloatingPointIeee754<TType>
    {
        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data * Math.Pow(LeftChildren.Data, RightChildren.Data - 1.0);
            RightChildren.Grad += Grad * Math.Pow(LeftChildren.Data, RightChildren.Data) * Math.Log(LeftChildren.Data);
        }
    }
}