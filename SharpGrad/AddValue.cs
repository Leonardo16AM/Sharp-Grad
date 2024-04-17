namespace SharpGrad.DifEngine
{
    public class AddValue : Value
    {
        public AddValue(Value left, Value right)
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