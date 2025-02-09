using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad
{
    public class Constant<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        private readonly Expression field;
        internal Expression GetExpression(Expression index)
        {
            if(this.Shape.Size() == 1)
            {
                return Expression.ArrayAccess(field, Expression.Constant(0));
            }
            else
            {
                return Expression.ArrayAccess(field, index);
            }
        }

        public Constant(TType[] data, Dimension[] shape, string name)
            : base(shape, name)
        {
            if(shape.Size() != data.Length)
            {
                throw new System.ArgumentException($"The shape size {shape.Size()} is not equal to the data length {data.Length}");
            }
            base.data = data;
            field = Expression.Field(Expression.Constant(this), nameof(data));
        }
        public Constant(TType data, string name)
            : this([data], [], name)
        { }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand)
        {
            operand = GetExpression(index);
            return true;
        }

        public static implicit operator Constant<TType>(TType d)
            => new(d, $"c{InstanceCount++}");
    }
}