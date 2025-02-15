using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class Reduce<TType> : Value<TType>
        where TType : INumber<TType>
    {
        protected Reduce(Dimension[] shape, string name, Value<TType> child)
            : base(shape, name, [child])
        {
            if (shape.All(e => child.Shape.Contains(e)))
            {
                throw new ArgumentException($"Shape of '{Name}' [{string.Join(", ", shape.AsEnumerable())}] is not a subset of '{child.Name}' shape [{string.Join(", ", child.Shape.AsEnumerable())}].");
            }
        }
    }
}
