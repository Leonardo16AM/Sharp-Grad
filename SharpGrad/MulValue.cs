namespace SharpGrad.DifEngine
{
    public class MulValue : Value
    {
        public MulValue(Value left, Value right)
            : base(left.Data * right.Data, "*", left, right)
        {
        }

        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data;
            RightChildren.Grad += Grad * LeftChildren.Data;
        }
    }
}