using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrad.NN
{
    public static partial class Loss
    {
        public static Value MSE(List<Value> Y, List<Value> Y_hat)
        {
            Value loss = Value.Zero;
            for (int i = 0; i < Y.Count; i++)
            {
                var nl = loss + ((Y[i] - Y_hat[i]) ^ 2.0);
                loss = nl;
            }
            return loss;
        }
    }
}
