using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public class TestTanh
    {
        public static void Tanh<T>()
            where T : IBinaryFloatingPointIeee754<T>, IExponentialFunctions<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            var c = a.Tanh();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = T.Tanh(T.CreateTruncating(1.5));
            Debug.Assert(c.Data[0] == r);

            a.Data[0] = T.CreateTruncating(2.0);
            cFunc();
            r = T.Tanh(T.CreateTruncating(2.0));
            Debug.Assert(c.Data[0] == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                cFunc();
                r = T.Tanh(aData);
                Debug.Assert(c.Data[0] == r);
            }
        }

        [TestMethod]
        public void TanhHalf() => Tanh<Half>();
        [TestMethod]
        public void TanhFloat() => Tanh<float>();
        [TestMethod]
        public void TanhDouble() => Tanh<double>();
    }
}
