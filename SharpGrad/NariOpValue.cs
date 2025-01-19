using SharpGrad.DifEngine;
using System;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad
{
    public abstract class NariOpValue<TType> : Value<TType>
    where TType : INumber<TType>
    {
        protected bool isComputed = false;
        public override TType Data {
            get
            {
                if(!isComputed)
                {
                    ForwardLambda();
                    isComputed = true;
                }
                return data;
            }
        }

        public NariOpValue(string name, params Value<TType>[] childs)
            : base(name, childs)
        {
            if (childs.Length < 1)
                throw new ArgumentException($"Operator {name} must have at least one child.");
        }
    }
}