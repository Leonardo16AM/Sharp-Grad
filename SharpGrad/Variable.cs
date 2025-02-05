using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class Variable<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public new TType[] Data { get => data; set => data = value; }

        public Variable(TType[] data, IReadOnlyList<Dimension> shape, string name, params Value<TType>[] childs)
            : base(shape, name, childs)
        {
            base.data = data;
        }

        public Variable(TType data, string name, params Value<TType>[] childs)
            : this([data], [], name, childs)
        { }

        public Variable(IReadOnlyList<Dimension> shape, string name, params Value<TType>[] childs) :
            this(new TType[shape.Size()], shape, name, childs)
        { }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand)
        {
            if (!variableExpressions.TryGetValue(this, out operand))
            {
                operand = Expression.Variable(typeof(TType), Name);
                variableExpressions[this] = operand;
                Expression field = Expression.Field(Expression.Constant(this), nameof(data));
                Expression arrayAccess = Expression.ArrayAccess(field, (Shape.Size() == 1) ? Expression.Constant(0) : index);
                forwardExpressionList.Add(Expression.Assign(operand, arrayAccess));
            }
            return true;
        }

        public static implicit operator Variable<TType>(TType d)
            => new(d, $"var{InstanceCount++}");

    }
}