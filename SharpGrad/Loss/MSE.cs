﻿using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static NariOperation<TType> MSE<TType>(Value<TType>[] Y, Value<TType>[] Y_hat)
            where TType : INumber<TType>
        {
            NariOperation<TType> loss = Y[0] - Y_hat[0];
            loss *= loss;
            TType count = TType.One;
            for (int i = 1; i < Y.Length; i++)
            {
                var nl = Y[i] - Y_hat[i];
                loss += nl * nl;
                count++;
            }
            return loss / count;
        }
    }
}
