namespace SharpGrad.DifEngine
{
    public class SubValue : Value
    {
        public SubValue(Value left, Value right)
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