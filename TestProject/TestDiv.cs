using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public class TestDiv
    {
        public static void Div<T>()
            where T : INumber<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            Variable<T> b = new(T.CreateTruncating(2.0), "b");
            var c = a / b;
            var cFunc = c.Forward;
            cFunc();
            Debug.Assert(c.Data[0] == T.CreateTruncating(0.75));
            a.Data[0] = T.CreateTruncating(2.0);
            b.Data[0] = T.CreateTruncating(3.0);
            cFunc();
            Debug.Assert(c.Data[0] == T.CreateTruncating(2.0) / T.CreateTruncating(3.0));
            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                var bData = Common.Random<T>() + T.One;
                b.Data[0] = bData;
                cFunc();
                Debug.Assert(c.Data[0] == aData / bData);
            }
        }

        [TestMethod]
        public void DivHalf() => Div<Half>();
        [TestMethod]
        public void DivFloat() => Div<float>();
        [TestMethod]
        public void DivDouble() => Div<double>();
        [TestMethod]
        public void DivDecimal() => Div<decimal>();

        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void DivByte() => Div<byte>();
        // The binary operator Add is not defined for the types 'System.SByte' and 'System.SByte'.
        //[TestMethod]
        //public void DivSByte() => Div<sbyte>();
        [TestMethod]
        public void DivShort() => Div<short>();
        [TestMethod]
        public void DivUShort() => Div<ushort>();
        [TestMethod]
        public void DivInt() => Div<int>();
        [TestMethod]
        public void DivUInt() => Div<uint>();
        [TestMethod]
        public void DivLong() => Div<long>();
        [TestMethod]
        public void DivULong() => Div<ulong>();
        [TestMethod]
        public void DivBigInteger() => Div<BigInteger>();
    }
}
