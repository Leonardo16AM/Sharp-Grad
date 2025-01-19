using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Constant<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public Constant(TType data, string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            base.data = data;
        }

        public override Expression GenerateForwardExpression()
        {
            return Expression.Constant(data);
        }

        public static implicit operator Constant<TType>(TType d)
            => new(d, $"c{InstanceCount++}");
    }
}