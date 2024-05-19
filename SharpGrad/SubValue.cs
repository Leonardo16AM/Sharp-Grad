using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class SubValue<TType> : Value<TType>
        where TType : IFloatingPointIeee754<TType>
    {
        public SubValue(Value<TType> left, Value<TType> right)
            : base(left.Data - right.Data, "-", left, right)
        {
        }
        protected override void Backward()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad -= Grad;
        }
    }
}