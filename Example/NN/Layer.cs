﻿using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.NN
{
    public class Layer<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public Neuron<TType>[] Neurons;
        public int NeuronsCount;
        public int Inputs;
        public bool ActFunc;

        public Layer(int neurons, int inputs, bool act_func)
        {
            Neurons = new Neuron<TType>[neurons];
            for (int i = 0; i < neurons; i++)
            {
                Neurons[i] = new Neuron<TType>(inputs, act_func);
            }
        }

        public Value<TType>[] Forward(Value<TType>[] X)
        {
            Value<TType>[] Y = new Value<TType>[Neurons.Length];
            for (int i = 0; i < Neurons.Length; i++)
            {
                Y[i] = Neurons[i].Forward(X);
            }
            return Y;
        }

        public void Step(TType lr)
        {
            foreach (Neuron<TType> n in Neurons)
            {
                n.Step(lr);
                n.Biai.Data -= lr * n.Biai.Grad;
            }
        }
    }
}