using System;

namespace SharpGrad.DifEngine
{
    public class PowValue : Value
    {
        public PowValue(Value left, Value right)
            : base(Math.Pow(left.Data, right.Data), "^", left, right)
        {
        }
        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data * Math.Pow(LeftChildren.Data, RightChildren.Data - 1.0);
            RightChildren.Grad += Grad * Math.Pow(LeftChildren.Data, RightChildren.Data) * Math.Log(LeftChildren.Data);
        }
    }
}