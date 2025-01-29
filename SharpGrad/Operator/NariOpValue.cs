using SharpGrad.DifEngine;
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
        protected bool isComputed = false;

        public delegate void ComputeGradientDelegate(
            Dictionary<Value<TType>, Expression> VariableExpressions,
            Dictionary<Value<TType>, Expression> GradientExpressions,
            List<Expression> expressionList);

        public abstract ComputeGradientDelegate[] ChildrensCompute { get; }

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

        internal abstract Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions);
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

        private List<Value<TType>>? topOSort = null;
        private readonly Dictionary<Value<TType>, Expression> variableExpressions = [];
        private readonly Dictionary<Value<TType>, Expression> gradientExpressions = [];


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
                            forwardExpressionList.Add(Expression.Assign(expression, Expression.Field(Expression.Constant(v), nameof(data))));
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

                    // Backup all parameters to data
                    List<ParameterExpression> parameters = [];
                    foreach (var e in variableExpressions)
                    {
                        if (e.Value is ParameterExpression parameter)
                        {
                            forwardExpressionList.Add(Expression.Assign(Expression.Field(Expression.Constant(e.Key), nameof(data)), parameter));
                            parameters.Add(parameter);
                        }
                    }

                    // Build block and compile Expression
                    Expression forwardExpression = Expression.Block(parameters, forwardExpressionList);
                    forwardLambda = Expression.Lambda<Action>(forwardExpression).Compile();
                }
                return forwardLambda;
            }
        }

        private Action? backwardLambda;
        public Action BackwardLambda
        {
            get
            {
                if (backwardLambda is null)
                {
                    _ = ForwardLambda;
                    gradientExpressions.Clear();
                    Grad = TType.One;
                    gradientExpressions.Add(this, Expression.Constant(TType.One));
                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                    }
                    List<Expression> backwardExpressionList = [];
                    // Load data
                    foreach (var e in variableExpressions)
                    {
                        if (e.Value is ParameterExpression parameter)
                        {
                            backwardExpressionList.Add(Expression.Assign(parameter, Expression.Field(Expression.Constant(e.Key), nameof(data))));
                        }
                    }

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
                            backwardExpressionList.Add(Expression.Assign(gradField, gradientExpressions[v]));
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

                    // Build block and compile Expression
                    Expression backwardExpression = Expression.Block(variableExpressions.Values.Union(gradientExpressions.Values).OfType<ParameterExpression>(), backwardExpressionList);
                    backwardLambda = Expression.Lambda<Action>(backwardExpression).Compile();
                }
                return backwardLambda;
            }
        }
    }
}