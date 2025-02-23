using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.DifEngine
{
    public abstract class Value<TType>
        where TType : INumber<TType>
    {
        protected static PropertyInfo thisIndexerProperty = typeof(Value<TType>).GetProperty("Item", typeof(TType))!;

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
        public virtual void Init() { }
        public Value(IReadOnlyList<Dimension> shape, string name, params Value<TType>[] childs)
        {
            Name = name;
            Shape = [.. shape.Except([Dimension.Scalar]).Distinct()];
            Operands = childs;
            int length = Size;
            data = new TType[length];
            gradient = new TType[length];
            Init();
        }

        public readonly Value<TType>[] Operands;
        public readonly string Name;
        protected TType[] data;
        public virtual TType[] Data => data;

        private bool IsShapeEqual(Dimension[] shape)
        {
            if (shape.Length != Shape.Length)
            {
                return false;
            }
            for (int i = 0; i < shape.Length; i++)
            {
                if (shape[i] != Shape[i])
                {
                    return false;
                }
            }
            return true;
        }

        private int[] GetLocalIndices(Dimdices indices)
        {
            if (indices.IsScalar)
            {
                return [0];
            }
            if (IsShapeEqual(indices.Shape))
            {
                return [.. indices.Indices];
            }
            else
            {
                int[] localIndice = new int[Shape.Length];
                for (int i = localIndice.Length - 1; i >= 0; i--)
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
        }

        public TType this[Dimdices indices]
        {
            get
            {
                int[] localIndices = GetLocalIndices(indices);
                long i = Shape.GetLinearIndex(localIndices);
                return data[i];
            }
            internal set
            {
                int[] localIndices = GetLocalIndices(indices);
                long i = Shape.GetLinearIndex(localIndices);
                data[i] = value;
            }
        }

        public Expression Get(Expression index)
            => Expression.MakeIndex(Expression.Constant(this), thisIndexerProperty, [index]);

        private TType[] gradient;
        public TType GetGradient(Dimdices indices)
        {
            int[] localIndices = GetLocalIndices(indices);
            long i = Shape.GetLinearIndex(localIndices);
            return gradient[i];
        }

        public void SetGradient(Dimdices indices, TType value)
        {
            int[] localIndices = GetLocalIndices(indices);
            long i = Shape.GetLinearIndex(localIndices);
            gradient[i] = value;
        }

        internal void InnerDFS(List<Value<TType>> topOSort, Dictionary<Value<TType>, int> usageCount)
        {
            if (usageCount.TryAdd(this, 0))
            {
                for (int i = 0; i < Operands.Length; i++)
                {
                    if (Operands[i] is ReduceOperation<TType> r)
                    {
                        if (usageCount.TryAdd(r, 0))
                        {
                            topOSort.Add(r);
                        }
                        usageCount[r]++;
                    }
                    else
                    {
                        Operands[i].InnerDFS(topOSort, usageCount);
                    }
                }
                topOSort.Add(this);
            }
            else
            {
                usageCount[this]++;
            }
        }

        private static int gradientCount = 0;
        internal void AssignGradientExpession(Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList, Expression index, Value<TType> LeftOperand, Expression gradientExpression)
        {
            if (!gradientExpressions.TryGetValue(LeftOperand, out Expression? leftGrad))
            {
                leftGrad = Expression.Variable(typeof(TType), $"grad{gradientCount++}");
                gradientExpressions[LeftOperand] = leftGrad;
                // Get the gradient of the left operand
                MethodInfo getGradientMethod = typeof(Value<TType>).GetMethod(nameof(GetGradient), [typeof(Dimdices)])!;
                Expression getGradientCall = Expression.Call(Expression.Constant(LeftOperand), getGradientMethod, index);
                // Assign the gradient to the left operand
                expressionList.Add(Expression.Assign(leftGrad, getGradientCall));
            }
            expressionList.Add(Expression.AddAssign(leftGrad, gradientExpression));


            // Add this grad to this value using a call to AddGradient
            MethodInfo addGradientMethod = typeof(Value<TType>).GetMethod(nameof(SetGradient), [typeof(Dimdices), typeof(TType)])!;
            Expression addGradientCall = Expression.Call(Expression.Constant(this), addGradientMethod, index, leftGrad);
            expressionList.Add(addGradientCall);
        }

        public abstract bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand);
        internal abstract Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index);
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

        protected void InitGradientForBackward()
        {
            Array.Fill(gradient, TType.One);
        }

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