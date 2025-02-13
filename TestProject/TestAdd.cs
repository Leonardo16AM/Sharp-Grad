using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestClass]
    public sealed class TestAdd
    {

        public static void Add<T>()
            where T : INumber<T>
        {
            Variable<T> a = new(T.CreateTruncating(1.5), "a");
            Variable<T> b = new(T.CreateTruncating(2.0), "b");
            var c = a + b;
            var cFunc = c.Forward;
            cFunc();
            Debug.Assert(c.Data[0] == T.CreateTruncating(3.5));

            a.Data[0] = T.CreateTruncating(2.0);
            b.Data[0] = T.CreateTruncating(3.0);
            cFunc();
            Debug.Assert(c.Data[0] == T.CreateTruncating(5.0));

            for (int i = 0; i < 10; i++)
            {
                var aData = Common.Random<T>();
                a.Data[0] = aData;
                var bData = Common.Random<T>();
                b.Data[0] = bData;
                cFunc();
                Debug.Assert(c.Data[0] == aData + bData);
            }
        }

        [TestMethod]
        public void AddHalf() => Add<Half>();

        [TestMethod]
        public void AddFloat() => Add<float>();

        [TestMethod]
        public void AddDouble() => Add<double>();

        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void AddByte() => Add<byte>();

        //The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void AddSByte() => Add<sbyte>();

        [TestMethod]
        public void AddShort() => Add<short>();

        [TestMethod]
        public void AddUShort() => Add<ushort>();

        [TestMethod]
        public void AddInt() => Add<int>();

        [TestMethod]
        public void AddUInt() => Add<uint>();

        [TestMethod]
        public void AddLong() => Add<long>();

        [TestMethod]
        public void AddULong() => Add<ulong>();

        [TestMethod]
        public void AddDecimal() => Add<decimal>();
    }
}
