using System;
using System.Diagnostics;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    //TODO: Use class inheritance instead of switch-case
    public class Value<TType>(TType data, string name, params ValueBase<TType>[] childs)
        : ValueBase<TType>(data, name, childs)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        private static int InstanceCount = 0;

        public static readonly Value<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Value<TType> Zero = new(TType.Zero, "0");

        public delegate void BackwardPass();

        #region BASIC ARITHMETIC OPERATIONS
        public static Value<TType> Add(Value<TType> left, Value<TType> right)
            => new AddValue<TType>(left, right);
        public static Value<TType> operator +(Value<TType> left, Value<TType> right)
            => Add(left, right);

        public static Value<TType> Sub(Value<TType> left, Value<TType> right)
            => new SubValue<TType>(left, right);
        public static Value<TType> operator -(Value<TType> left, Value<TType> right)
            => Sub(left, right);
        public static Value<TType> Sub(Value<TType> @this)
            => new SubValue<TType>(Zero, @this);
        public static Value<TType> operator -(Value<TType> @this)
            => Sub(@this);


        public static Value<TType> Mul(Value<TType> left, Value<TType> right)
            => new MulValue<TType>(left, right);
        public static Value<TType> operator *(Value<TType> left, Value<TType> right)
            => Mul(left, right);

        public static Value<TType> Div(Value<TType> left, Value<TType> right)
            => new DivValue<TType>(left, right);
        public static Value<TType> operator /(Value<TType> left, Value<TType> right)
            => Div(left, right);


        public static Value<TType> Pow(Value<TType> left, Value<TType> right)
            => new PowValue<TType>(left, right);
        public Value<TType> Pow(Value<TType> other)
            => new PowValue<TType>(this, other);
        #endregion

        #region ACTIVATION FUNCTIONS

        public Value<TType> ReLU()
            => new ReLUValue<TType>(this);

        public Value<TType> Tanh()
            => new TanhValue<TType>(this);

        public Value<TType> Sigmoid()
            => new SigmoidValue<TType>(this);
        
        public Value<TType> LeakyReLU(TType alpha)
            => new LeakyReLUValue<TType>(this,alpha);
        #endregion

        public static implicit operator Value<TType>(TType d)
            => new(d, $"value_{++InstanceCount}");
        public static explicit operator TType(Value<TType> v)
            => v.Data;
    }
}