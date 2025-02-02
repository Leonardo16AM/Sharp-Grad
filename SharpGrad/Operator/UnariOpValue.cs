using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class UnariOpValue<TType> :
        NariOpValue<TType>
        where TType : INumber<TType>
    {
        public Value<TType> Operand => Operands[0];

        public sealed override ComputeGradientDelegate[] ChildrensCompute { get; }

        public UnariOpValue(string name, Value<TType> child) : base(name, child)
        {
            ChildrensCompute = [ComputeGradient];
        }

        public override string ToString()
            => $"{Name}({Operand})";
        protected abstract void ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);
    }
}