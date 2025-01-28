using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class NariOpValue<TType> : Value<TType>
    where TType : INumber<TType>
    {
        protected bool isComputed = false;
        public override TType Data
        {
            get
            {
                //if (!isComputed)
                //{
                //    ForwardLambda();
                //    isComputed = true;
                //}
                return data;
            }
        }

        public NariOpValue(string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            if (childs.Length < 1)
                throw new ArgumentException($"Operator {name} must have at least one child.");
        }

        protected abstract Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions);
        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, out Expression? operand)
        {
            if (!variableExpressions.TryGetValue(this, out operand))
            {
                for (int i = 0; i < Childrens.Length; i++)
                {
                    Childrens[i].BuildForward(variableExpressions, forwardExpressionList);
                }
                operand = Expression.Variable(typeof(TType), Name);
                variableExpressions[this] = operand;
                forwardExpressionList.Add(Expression.Assign(operand, GetForwardComputation(variableExpressions)));
            }
            return true;
        }

        public void Backpropagate(int accCount = 1)
        {
            Grad = TType.One;
            List<Value<TType>> TopOSort = [];
            HashSet<Value<TType>> Visited = [];
            DFS(TopOSort, Visited);
            TType accC = TType.CreateSaturating(accCount);
            for (int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward(accC);
            }
        }


    }
}