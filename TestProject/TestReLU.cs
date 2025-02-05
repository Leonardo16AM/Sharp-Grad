using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public class TestReLU
    {
        public static void ReLU<T>()
            where T : INumber<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            var c = a.ReLU();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = T.Max(T.Zero, T.CreateTruncating(1.5));
            Debug.Assert(c.Data[0] == r);

            a.Data[0] = T.CreateTruncating(-2.0);
            cFunc();
            r = T.Max(T.Zero, T.CreateTruncating(-2.0));
            Debug.Assert(c.Data[0] == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                cFunc();
                r = T.Max(T.Zero, aData);
                Debug.Assert(c.Data[0] == r);
            }
        }

        [TestMethod]
        public void ReLUHalf() => ReLU<Half>();
        [TestMethod]
        public void ReLUFloat() => ReLU<float>();
        [TestMethod]
        public void ReLUDouble() => ReLU<double>();
        [TestMethod]
        public void ReLUDecimal() => ReLU<decimal>();

        [TestMethod]
        public void ReLUByte() => ReLU<byte>();
        [TestMethod]
        public void ReLUSByte() => ReLU<sbyte>();
        [TestMethod]
        public void ReLUShort() => ReLU<short>();
        [TestMethod]
        public void ReLUUShort() => ReLU<ushort>();
        [TestMethod]
        public void ReLUInt() => ReLU<int>();
        [TestMethod]
        public void ReLUUInt() => ReLU<uint>();
        [TestMethod]
        public void ReLULong() => ReLU<long>();
        [TestMethod]
        public void ReLUULong() => ReLU<ulong>();
        [TestMethod]
        public void ReLUBigInteger() => ReLU<BigInteger>();
    }
}
