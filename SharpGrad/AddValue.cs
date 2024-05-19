using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class AddValue<TType> : Value<TType>
        where TType : IFloatingPointIeee754<TType>
    {
        public AddValue(Value<TType> left, Value<TType> right)
            : base(left.Data + right.Data, "+", left, right)
        {
        }

        protected override void Backward()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad += Grad;
        }
    }
}