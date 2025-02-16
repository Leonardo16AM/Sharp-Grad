using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class ReduceOperation<TType> : NariOperation<TType>
        where TType : INumber<TType>
    {
        public override ComputeGradientDelegate[] ChildrensCompute { get; }

        public Value<TType> Operand => Operands[0];
        internal abstract Expression GetForwardComputationEnding(
            Dictionary<Value<TType>, Expression> variableExpressions,
            List<Expression> forwardExpressionList,
            Expression index);

        public abstract Expression ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        protected ReduceOperation(Dimension[] shape, string name, Value<TType> child)
            : base(shape, name, [child])
        {
            if (!shape.All(e => child.Shape.Contains(e)))
            {
                throw new ArgumentException($"Shape of '{Name}' [{string.Join(", ", shape.AsEnumerable())}] is not a subset of '{child.Name}' shape [{string.Join(", ", child.Shape.AsEnumerable())}].");
            }
            ChildrensCompute = [ComputeGradient];
        }
    }
}
