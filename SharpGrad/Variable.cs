using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.DifEngine
{
    public class Variable<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        public new TType this[Dimdices indices] { get =>base[indices]; set => base[indices] = value; }

        public Variable(TType[] data, Dimension[] shape, string name)
            : base(shape, name)
        {
            if (shape.Size() != data.Length)
            {
                throw new System.ArgumentException($"The shape size {shape.Size()} is not equal to the data length {data.Length}");
            }
            base.data = data;
        }

        public Variable(TType data, string name)
            : this([data], [], name)
        { }

        public Variable(Dimension[] shape, string name) :
            this(new TType[shape.Size()], shape, name)
        { }

        public Variable(Dimension shape, string name) :
            this([shape], name)
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
        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index)
        {
            Expression variable = Expression.Variable(typeof(TType), Name);
            variableExpressions[this] = variable;
            forwardExpressionList.Add(Expression.Assign(variable, Get(index)));
            return variable;
        }

        public static implicit operator Variable<TType>(TType d)
            => new(d, $"var{InstanceCount++}");

    }
}