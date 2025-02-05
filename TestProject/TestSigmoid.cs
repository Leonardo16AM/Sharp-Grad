using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public class TestSigmoid
    {
        public static void Sigmoid<T>()
            where T : IBinaryFloatingPointIeee754<T>, IExponentialFunctions<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            var c = a.Sigmoid();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = T.One / (T.One + T.Exp(T.CreateTruncating(-1.5)));
            Debug.Assert(c.Data[0] == r);

            a.Data[0] = T.CreateTruncating(2.0);
            cFunc();
            r = T.One / (T.One + T.Exp(T.CreateTruncating(-2.0)));
            Debug.Assert(c.Data[0] == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                cFunc();
                r = T.One / (T.One + T.Exp(T.CreateTruncating(-aData)));
                Debug.Assert(c.Data[0] == r);
            }
        }

        [TestMethod]
        public void SigmoidHalf() => Sigmoid<Half>();
        [TestMethod]
        public void SigmoidFloat() => Sigmoid<float>();
        [TestMethod]
        public void SigmoidDouble() => Sigmoid<double>();
    }
}
