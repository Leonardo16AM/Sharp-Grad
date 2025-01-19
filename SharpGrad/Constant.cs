using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Constant<TType>(TType data, string name, params Value<TType>[] childs)
        : Value<TType>(data, name, childs)
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public override Expression GenerateForwardExpression()
        {
            return Expression.Constant(Data);
        }

        public static implicit operator Constant<TType>(TType d)
            => new(d, $"c{InstanceCount++}");
    }
}