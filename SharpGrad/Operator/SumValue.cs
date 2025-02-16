using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.Operator
{
    public class SumValue<TType> : ReduceOperation<TType>
        where TType : INumber<TType>
    {
        public override void Init()
        {
            Array.Fill(data, TType.AdditiveIdentity);
        }

        public SumValue(Dimension[] shape, string name, Value<TType> child)
            : base(shape, name, child)
        {
            if (!shape.All(e => child.Shape.Contains(e)))
            {
                throw new ArgumentException($"Shape of '{Name}' [{string.Join(", ", shape.AsEnumerable())}] is not a subset of '{child.Name}' shape [{string.Join(", ", child.Shape.AsEnumerable())}].");
            }
        }

        public override bool GetAsOperand(
            Dictionary<Value<TType>, Expression> variableExpressions,
            List<Expression> forwardExpressionList, Expression index,
            out Expression? operand)
        {
            throw new NotImplementedException();
        }

        internal override Expression GetForwardComputation(
            Dictionary<Value<TType>, Expression> variableExpressions,
            List<Expression> forwardExpressionList, 
            Expression index)
        {
            Expression operand = variableExpressions[Operands[0]];
            Expression thisExpression = Expression.Constant(this);
            PropertyInfo thisItem = typeof(Value<TType>).GetProperty("Item")!;
            Expression accumulator = Expression.MakeIndex(thisExpression, thisItem, [index]);
            Expression addAssign = Expression.AddAssign(accumulator, operand);
            forwardExpressionList.Add(addAssign);
            return accumulator;
        }

        protected override Expression ComputeGradient(
            Dictionary<Value<TType>, Expression> VariableExpressions,
            Dictionary<Value<TType>, Expression> GradientExpressions,
            List<Expression> expressionList)
        {
            return GradientExpressions[this];
        }
    }
}
