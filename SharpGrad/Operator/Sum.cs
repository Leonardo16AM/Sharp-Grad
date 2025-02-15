using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.Operator
{
    public class Sum<TType> : Reduce<TType>
        where TType : INumber<TType>
    {
        public void Init()
        {
            Array.Fill(data, TType.AdditiveIdentity);
        }

        public Sum(Dimension[] shape, string name, Value<TType> child) : base(shape, name, child)
        {
        }

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand)
        {
            throw new NotImplementedException();
        }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index)
        {
            Expression operand = variableExpressions[Operands[0]];
            Expression thisExpression = Expression.Constant(this);
            PropertyInfo thisItem = typeof(Value<TType>).GetProperty("Item")!;
            Expression accumulator = Expression.MakeIndex(thisExpression, thisItem, [index]);
            Expression addAssign = Expression.AddAssign(accumulator, operand);
            forwardExpressionList.Add(addAssign);
            return accumulator;
        }
    }
}
