using SharpGrad.Operator;
using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.DifEngine
{
    public abstract class Value<TType>
        where TType : INumber<TType>
    {
        public Dimension[] Shape { get; private set; }
        public int Size => Shape.Size();
        public bool IsScalar => Shape.IsScalar();

        /// <summary>
        /// If true, this value is an output of the computation graph and should be saved back to its data field.
        /// </summary>
        public virtual bool IsOutput { get; set; } = false;

        public static readonly Expression ExpressionZero = Expression.Constant(TType.Zero);
        public static readonly Expression ExpressionOne = Expression.Constant(TType.One);

        private static int InstanceCount = 0;
        public static readonly Constant<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Constant<TType> Zero = new(TType.Zero, "0");

        public Value(Dimension[] shape, string name, params Value<TType>[] childs)
        {
            Name = name;
            Shape = shape;
            Operands = childs;
            int length = Size;
            data = new TType[length];
            gradient = new TType[length];
        }

        public readonly Value<TType>[] Operands;
        public readonly string Name;
        protected TType[] data;
        public virtual TType[] Data => data;

        private int[] GetLocalIndices(Dimdices indices)
        {
            if (indices.IsScalar)
            {
                throw new ArgumentException("Trying to access a non-scalar value with scalar indices.");
            }
            int[] localIndice = new int[Shape.Length];
            for (int i = localIndice.Length -1; i >= 0; i--)
            {
                Dimension dim = Shape[i];
                Index index = indices[dim];
                int idx = index.Value;
                if (index.IsFromEnd)
                {
                    if (idx > dim.Size)
                    {
                        throw new IndexOutOfRangeException($"Index {idx} is out of range for dimension {dim.Size}");
                    }
                    localIndice[i] = dim.Size - idx;
                }
                else
                {
                    if (idx >= dim.Size)
                    {
                        throw new IndexOutOfRangeException($"Index {idx} is out of range for dimension {dim.Size}");
                    }
                    localIndice[i] = idx;
                }
            }
            return localIndice;
        }

        public TType this[Dimdices indices]
        {
            get
            {
                if(IsScalar)
                {
                    return data[0];
                }
                int[] localIndices = GetLocalIndices(indices);
                long i = Shape.GetLinearIndex(localIndices);
                return data[i];
            }
            set
            {
                if (IsScalar)
                {
                    data[0] = value;
                    return;
                }
                int[] localIndices = GetLocalIndices(indices);
                long i = Shape.GetLinearIndex(localIndices);
                data[i] = value;
            }
        }

        private TType[] gradient;
        public TType GetGradient(Dimdices indices)
        {
            if (IsScalar)
            {
                return gradient[0];
            }
            int[] localIndices = GetLocalIndices(indices);
            long i = Shape.GetLinearIndex(localIndices);
            return gradient[i];
        }

        public void SetGradient(Dimdices indices, TType value)
        {
            if (IsScalar)
            {
                gradient[0] = value;
                return;
            }
            int[] localIndices = GetLocalIndices(indices);
            long i = Shape.GetLinearIndex(localIndices);
            gradient[i] = value;
        }
        public void ClearGradient()
        {
            Array.Fill(gradient, TType.Zero);
        }

        protected void DFS(List<Value<TType>> TopOSort, HashSet<Value<TType>> Visited)
        {
            if (Visited.Add(this))
            {
                foreach (var child in Operands)
                {
                    child.DFS(TopOSort, Visited);
                }
                TopOSort.Add(this);
            }
        }

        public abstract bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand);
        public void BuildForward(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index)
            => _ = GetAsOperand(variableExpressions, forwardExpressionList, index, out var _);
        public Expression GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, Expression index)
        {
            List<Expression> forwardExpressionList = [];
            if (GetAsOperand(variableExpressions, forwardExpressionList, index, out var operand)
                && forwardExpressionList.Count == 0)
            {
                return operand!;
            }
            else
            {
                throw new InvalidOperationException($"Expression list should be empty. Found {forwardExpressionList.Count} expressions.");
            }
        }
        protected void AssignGradientExpession(Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> forwardExpressionList, Value<TType> LeftOperand, Expression gradientExpression)
        {
            if (!gradientExpressions.TryGetValue(LeftOperand, out Expression? leftGrad))
            {
                leftGrad = Expression.Variable(typeof(TType), $"{LeftOperand.Name}_grad");
                gradientExpressions[LeftOperand] = leftGrad;
                forwardExpressionList.Add(Expression.Assign(leftGrad, gradientExpression));
            }
            else
            {
                forwardExpressionList.Add(Expression.AddAssign(leftGrad, gradientExpression));
            }
        }

        public override string ToString() => Name;

        #region BASIC ARITHMETIC OPERATIONS
        public static AddValue<TType> Add(Value<TType> left, Value<TType> right) => new(left, right);
        public static AddValue<TType> operator +(Value<TType> left, Value<TType> right) => Add(left, right);

        public static NegValue<TType> Neg(Value<TType> operand) => new(operand);
        public static NegValue<TType> operator -(Value<TType> operand) => Neg(operand);

        public static SubValue<TType> Sub(Value<TType> left, Value<TType> right) => new(left, right);
        public static SubValue<TType> operator -(Value<TType> left, Value<TType> right) => Sub(left, right);

        public static MulValue<TType> Mul(Value<TType> left, Value<TType> right) => new(left, right);
        public static MulValue<TType> operator *(Value<TType> left, Value<TType> right) => Mul(left, right);

        public static DivValue<TType> Div(Value<TType> left, Value<TType> right) => new(left, right);

        public void ResetGradient()
        {
            Array.Clear(gradient);
            if (Operands.Length > 0)
            {
                foreach (var child in Operands)
                {
                    child.ResetGradient();
                }
            }
        }

        public static DivValue<TType> operator /(Value<TType> left, Value<TType> right) => Div(left, right);
        #endregion

        public static implicit operator Value<TType>(TType d)
            => new Constant<TType>(d, $"v{InstanceCount++}");
    }
}