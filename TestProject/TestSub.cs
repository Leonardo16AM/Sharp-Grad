using SharpGrad.DifEngine;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public sealed class TestSub
    {
        public static void Sub<T>()
            where T : INumber<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.0), "a");
            Variable<T> b = new(T.CreateTruncating(2.0), "b");
            var c = a - b;
            var cFunc = c.Forward;
            cFunc();
            Assert.AreEqual(c.Data[0], T.CreateTruncating(1.0) - T.CreateTruncating(2.0));

            a.Data[0] = T.CreateTruncating(5.0);
            b.Data[0] = T.CreateTruncating(3.0);
            cFunc();
            Assert.AreEqual(c.Data[0], T.CreateTruncating(5.0) - T.CreateTruncating(3.0));

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                var bData = Common.Random<T>();
                b.Data[0] = bData;
                cFunc();
                Assert.AreEqual(c.Data[0], aData - bData);
            }
        }

        [TestMethod]
        public void SubHalf() => Sub<Half>();
        [TestMethod]
        public void SubFloat() => Sub<float>();
        [TestMethod]
        public void SubDouble() => Sub<double>();
        [TestMethod]
        public void SubDecimal() => Sub<decimal>();

        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void SubByte() => Sub<byte>();
        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void SubSByte() => Sub<sbyte>();
        [TestMethod]
        public void SubShort() => Sub<short>();
        [TestMethod]
        public void SubUShort() => Sub<ushort>();
        [TestMethod]
        public void SubInt() => Sub<int>();
        [TestMethod]
        public void SubUInt() => Sub<uint>();
        [TestMethod]
        public void SubLong() => Sub<long>();
        [TestMethod]
        public void SubULong() => Sub<ulong>();
        [TestMethod]
        public void SubBigInteger() => Sub<BigInteger>();
    }
}
