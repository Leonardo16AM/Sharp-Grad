using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using SharpGrad.Operator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class BinaryOperation<TType> : NariOperation<TType>
        where TType : INumber<TType>
    {
        public Value<TType> LeftOperand => Operands[0];
        protected abstract Expression ComputeLeftGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public Value<TType> RightOperand => Operands[1];
        protected abstract Expression ComputeRightGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public sealed override ComputeGradientDelegate[] ChildrensCompute { get; }

        public BinaryOperation(string name, Value<TType> left, Value<TType> right)
            : base(name, left, right)
        {
            ChildrensCompute = [ComputeLeftGradient, ComputeRightGradient];
        }

        protected abstract Expr GetForwardComputation(Expr left, Expr right);
        internal sealed override Expression GetForwardComputation(
            Dictionary<Value<TType>, Expression> variableExpressions, 
            List<Expression> forwardExpressionList,
            Expression index)
        {
            Expression left = LeftOperand.GetAsOperand(variableExpressions, index);
            Expression right = RightOperand.GetAsOperand(variableExpressions, index);
            Expression variable = Expression.Variable(typeof(TType), Name);
            variableExpressions[this] = variable;
            forwardExpressionList.Add(Expression.Assign(variable, GetForwardComputation(left, right)));
            return variable;
        }

        internal override void DFS(List<Value<TType>> topOSort, Dictionary<Value<TType>, int> usageCount)
        {
            if (usageCount.TryAdd(this, 0))
            {
                LeftOperand.DFS(topOSort, usageCount);
                RightOperand.DFS(topOSort, usageCount);
                topOSort.Add(this);
            }
            else
            {
                usageCount[this]++;
            }
        }

        public override string ToString()
            => $"({LeftOperand} {Name} {RightOperand})";
    }
}