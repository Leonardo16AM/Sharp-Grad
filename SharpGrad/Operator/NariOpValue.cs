using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class NariOpValue<TType> : Value<TType>
    where TType : INumber<TType>
    {
        public delegate void ComputeGradientDelegate(
            Dictionary<Value<TType>, Expression> VariableExpressions,
            Dictionary<Value<TType>, Expression> GradientExpressions,
            List<Expression> expressionList);

        public abstract ComputeGradientDelegate[] ChildrensCompute { get; }

        public override TType[] Data
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

        public NariOpValue(int shape, string name, params Value<TType>[] childs)
            : base(shape, name, childs)
        {
            if (childs.Length < 1)
                throw new ArgumentException($"Operator {name} must have at least one child.");
        }

        internal abstract Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions);
        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, out Expression? operand)
        {
            if (!variableExpressions.TryGetValue(this, out operand))
            {
                for (int i = 0; i < Operands.Length; i++)
                {
                    Operands[i].BuildForward(variableExpressions, forwardExpressionList);
                }
                operand = Expression.Variable(typeof(TType), Name);
                variableExpressions[this] = operand;
                forwardExpressionList.Add(Expression.Assign(operand, GetForwardComputation(variableExpressions)));
            }
            return true;
        }

        private List<Value<TType>>? topOSort = null;
        private readonly Dictionary<Value<TType>, Expression> variableExpressions = [];
        private readonly Dictionary<Value<TType>, Expression> gradientExpressions = [];

        public static List<Expression> BuildForwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, List<Value<TType>> topOSort)
        {
            List<Expression> forwardExpressionList = [];

            for (int i = 0; i < topOSort.Count; i++)
            {
                Value<TType> e = topOSort[i];
                Debug.Assert(!variableExpressions.ContainsKey(e));
                if (e is Constant<TType> c)
                {
                    variableExpressions[c] = c.Expression;
                }
                else if (e is Variable<TType> v)
                {
                    Expression expression = Expression.Variable(typeof(TType), v.Name);
                    variableExpressions[v] = expression;
                    Expression field = Expression.Field(Expression.Constant(v), nameof(data));
                    // TODO: !!! DON'T USE Expression.Constant(0) !!!
                    Expression element = Expression.ArrayAccess(field, Expression.Constant(0));
                    forwardExpressionList.Add(Expression.Assign(expression, element));
                }
                else if (topOSort[i] is NariOpValue<TType> n)
                {
                    Expression expression = Expression.Variable(typeof(TType), n.Name);
                    variableExpressions[n] = expression;
                    forwardExpressionList.Add(Expression.Assign(expression, n.GetForwardComputation(variableExpressions)));
                }
                else
                {
                    throw new InvalidOperationException("Unknown type");
                }
            }

            return forwardExpressionList;
        }

        public static List<ParameterExpression> SaveParameters(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, bool all = true)
        {
            List<ParameterExpression> parameters = [];

            foreach (var e in variableExpressions)
            {
                if (e.Value is ParameterExpression parameter)
                {
                    if (all || e.Key.IsOutput)
                    {
                        Expression field = Expression.Field(Expression.Constant(e.Key), nameof(data));
                        // TODO: !!! DON'T USE Expression.Constant(0) !!!
                        Expression arrayAccess = Expression.ArrayAccess(field, Expression.Constant(0));
                        forwardExpressionList.Add(Expression.Assign(arrayAccess, parameter));
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters;
        }

        private Action? forwardLambda;
        public Action ForwardLambda
        {
            get
            {
                if (forwardLambda is null)
                {
                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                    }
                    List<Expression> forwardExpressionList = BuildForwardExpressionList(variableExpressions, topOSort);

                    // Backup all parameters to data
                    List<ParameterExpression> parameters = SaveParameters(variableExpressions, forwardExpressionList);

                    // Build block and compile Expression
                    Expression forwardExpression = Expression.Block(parameters, forwardExpressionList);
                    forwardLambda = Expression.Lambda<Action>(forwardExpression).Compile();
                }
                return forwardLambda;
            }
        }


        public static List<Expression> BuildBackwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Value<TType>> topOSort)
        {
            List<Expression> backwardExpressionList = [];
            for (int i = topOSort.Count - 1; i >= 0; i--)
            {
                Value<TType> e = topOSort[i];
                if (e is Constant<TType> c)
                {
                    continue;
                }
                else if (e is Variable<TType> v)
                {
                    // This is the last use of the variable.
                    // Save the gradient to Grad field.
                    Expression gradField = Expression.Field(Expression.Constant(v), nameof(Grad));
                    // TODO: !!! DON'T USE Expression.Constant(0) !!!
                    Expression arrayAccess = Expression.ArrayAccess(gradField, Expression.Constant(0));
                    backwardExpressionList.Add(Expression.Assign(arrayAccess, gradientExpressions[v]));
                }
                else if (topOSort[i] is NariOpValue<TType> n)
                {
                    for (int j = 0; j < n.ChildrensCompute.Length; j++)
                    {
                        n.ChildrensCompute[j](variableExpressions, gradientExpressions, backwardExpressionList);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unknown type");
                }
            }
            return backwardExpressionList;
        }

        private Action? backwardLambda;
        public Action BackwardLambda
        {
            get
            {
                if (backwardLambda is null)
                {
                    Array.Fill(Grad, TType.Zero);
                    gradientExpressions.Clear();
                    List<Expression> backwardExpressionList = [];
                    gradientExpressions.Add(this, Expression.Constant(TType.One));
                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                        backwardExpressionList.AddRange(BuildForwardExpressionList(variableExpressions, topOSort));
                        SaveParameters(variableExpressions, backwardExpressionList, false);
                    }
                    else
                    {
                        // Load data
                        foreach (var e in variableExpressions)
                        {
                            if (e.Value is ParameterExpression parameter)
                            {
                                backwardExpressionList.Add(Expression.Assign(parameter, Expression.Field(Expression.Constant(e.Key), nameof(data))));
                            }
                        }
                    }

                    backwardExpressionList.AddRange(BuildBackwardExpressionList(variableExpressions, gradientExpressions, topOSort));

                    // Build block and compile Expression
                    Expression backwardExpression = Expression.Block(variableExpressions.Values.Union(gradientExpressions.Values).OfType<ParameterExpression>(), backwardExpressionList);
                    backwardLambda = Expression.Lambda<Action>(backwardExpression).Compile();
                }
                return backwardLambda;
            }
        }
    }
}