using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public sealed class TestPow
    {
        public static void Pow<T>()
            where T : IBinaryFloatingPointIeee754<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            Variable<T> b = new(T.CreateTruncating(2.0), "b");
            var c = a.Pow(b);
            var cFunc = c.Forward;
            cFunc();
            Debug.Assert(c.Data[0] == T.Pow(T.CreateTruncating(1.5), T.CreateTruncating(2.0)));

            a.Data[0] = T.CreateTruncating(2.0);
            b.Data[0] = T.CreateTruncating(3.0);
            cFunc();
            Debug.Assert(c.Data[0] == T.Pow(T.CreateTruncating(2.0), T.CreateTruncating(3.0)));

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                var bData = Common.Random<T>();
                b.Data[0] = bData;
                cFunc();
                var r = T.Pow(aData, bData);
                Debug.Assert(c.Data[0] == r);
            }
        }

        [TestMethod]
        public void PowHalf() => Pow<Half>();
        [TestMethod]
        public void PowFloat() => Pow<float>();
        [TestMethod]
        public void PowDouble() => Pow<double>();
    }
}
