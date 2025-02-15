using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Xml.Linq;

namespace SharpGrad
{
    public class Constant<TType> : Value<TType>
        where TType : INumber<TType>
    {
        private static int InstanceCount = 0;

        private readonly Expression thisExpression;

        public Constant(TType[] data, Dimension[] shape, string name)
            : base(shape, name)
        {
            if(shape.Size() != data.Length)
            {
                throw new System.ArgumentException($"The shape size {shape.Size()} is not equal to the data length {data.Length}");
            }
            base.data = data;
            thisExpression = Expression.Constant(this);
            IsOutput = false;
        }

        public Constant(TType data, string name)
            : this([data], [], name)
        { }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand)
        {
            if (!variableExpressions.TryGetValue(this, out operand))
            {
                operand = GetForwardComputation(variableExpressions, forwardExpressionList, index);
            }
            return true;
        }

        public static implicit operator Constant<TType>(TType d)
            => new(d, $"c{InstanceCount++}");

        public override string ToString()
        {
            if(Shape.IsScalar())
            {
                return data[0].ToString()!;
            }
            else
            {
                return '[' + String.Join(", ", data) + ']';
            }
        }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index)
        {
            Expression variable = Expression.Variable(typeof(TType), ToString().Replace(" ", string.Empty));
            variableExpressions[this] = variable;
            forwardExpressionList.Add(Expression.Assign(variable, Get(index)));
            return variable;
        }
    }
}