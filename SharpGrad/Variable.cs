using SharpGrad.Activation;
using SharpGrad.Operators;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Variable<TType>(TType data, string name, params Value<TType>[] childs)
        : Value<TType>(data, name, childs)
        where TType : INumber<TType>
    {
        public override Expression GenerateForwardExpression()
        {
            return Expression.Field(Expression.Constant(this), nameof(Data));
        }
    }

    public class Constant<TType>(TType data, string name, params Value<TType>[] childs)
        : Value<TType>(data, name, childs)
        where TType : INumber<TType>
    {
        public override Expression GenerateForwardExpression()
        {
            return Expression.Constant(Data);
        }
    }
}