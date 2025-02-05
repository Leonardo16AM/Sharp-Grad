using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public class TestLeakyReLU
    {
        public static void LeakyReLU<T>()
            where T : INumber<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            var c = a.LeakyReLU(T.CreateTruncating(0.1));
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = T.Max(T.CreateTruncating(0.1) * T.CreateTruncating(1.5), T.CreateTruncating(1.5));
            Debug.Assert(c.Data[0] == r);
            a.Data[0] = T.CreateTruncating(-2.0);
            cFunc();
            r = T.Max(T.CreateTruncating(0.1) * T.CreateTruncating(-2.0), T.CreateTruncating(-2.0));
            Debug.Assert(c.Data[0] == r);
            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                cFunc();
                r = T.Max(T.CreateTruncating(0.1) * aData, aData);
                Debug.Assert(c.Data[0] == r);
            }
        }

        [TestMethod]
        public void LeakyReLUHalf() => LeakyReLU<Half>();
        [TestMethod]
        public void LeakyReLUFloat() => LeakyReLU<float>();
        [TestMethod]
        public void LeakyReLUDouble() => LeakyReLU<double>();

    }
}
