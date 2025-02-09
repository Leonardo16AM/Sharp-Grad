using SharpGrad;
using SharpGrad.DifEngine;
using System.Diagnostics;
using System.Numerics;

namespace TestProject
{
    [TestCategory("Array")]
    [TestClass]
    public sealed class TestAddArray
    {
        public static void Add<T>()
            where T : INumber<T>
        {
            T _oneAndHalf = T.CreateTruncating(1.5);
            T _two = T.CreateTruncating(2.0);
            T _three = T.CreateTruncating(3.0);
            T _four = T.CreateTruncating(4.0);

            Dimension dim = new(3);
            Dimension[] shape = [dim];
            Variable<T> a = new([_oneAndHalf, _two, _three], shape, "a");
            Variable<T> b = new([_two, _three, _four], shape, "b");
            Variable<T> c = new(_two, "scalar");

            Debug.WriteLine("d[3] = a[3] + b[3]");
            var d = a + b;
            var dFunc = d.ForwardLambda;
            dFunc();
            Debug.Assert(d.Data[0] == T.CreateTruncating(3.5), $"d[0] = {d.Data[0]}. Expected {T.CreateTruncating(3.5)}");
            Debug.WriteLine($"d[0] = {d.Data[0]} Expected {T.CreateTruncating(3.5)}");
            Debug.Assert(d.Data[1] == T.CreateTruncating(5.0), $"d[1] = {d.Data[1]}. Expected  {T.CreateTruncating(5.0)}");
            Debug.WriteLine($"d[1] = {d.Data[1]}. Expected  {T.CreateTruncating(5.0)}");
            Debug.Assert(d.Data[2] == T.CreateTruncating(7.0), $"d[2] = {d.Data[2]}. Expected {T.CreateTruncating(7.0)}");
            Debug.WriteLine($"d[2] = {d.Data[2]} Expected {T.CreateTruncating(7.0)}");

            Debug.WriteLine("e = d[3] + c");
            var e = d + c;
            var eFunc = e.ForwardLambda;
            eFunc();
            Debug.Assert(e.Data[0] == T.CreateTruncating(5.5), $"e[0] = {e.Data[0]}. Expected {T.CreateTruncating(5.5)}");
            Debug.WriteLine($"e[0] = {e.Data[0]} Expected {T.CreateTruncating(5.5)}");
            Debug.Assert(e.Data[1] == T.CreateTruncating(7.0), $"e[1] = {e.Data[1]}. Expected {T.CreateTruncating(7.0)}");
            Debug.WriteLine($"e[1] = {e.Data[1]} Expected {T.CreateTruncating(7.0)}");
            Debug.Assert(e.Data[2] == T.CreateTruncating(9.0), $"e[2] = {e.Data[2]}. Expected {T.CreateTruncating(9.0)}");
            Debug.WriteLine($"e[2] = {e.Data[2]} Expected {T.CreateTruncating(9.0)}");

            Debug.WriteLine("f[3] = c + d[3]");
            var f = c + d;
            var fFunc = f.ForwardLambda;
            fFunc();
            Debug.Assert(f.Data[0] == T.CreateTruncating(5.5), $"f[0] = {f.Data[0]}. Expected {T.CreateTruncating(5.5)}");
            Debug.WriteLine($"f[0] = {f.Data[0]} Expected {T.CreateTruncating(5.5)}");
            Debug.Assert(f.Data[1] == T.CreateTruncating(7.0), $"f[1] = {f.Data[1]}. Expected {T.CreateTruncating(7.0)}");
            Debug.WriteLine($"f[1] = {f.Data[1]} Expected {T.CreateTruncating(7.0)}");
            Debug.Assert(f.Data[2] == T.CreateTruncating(9.0), $"f[2] = {f.Data[2]}. Expected {T.CreateTruncating(9.0)}");
            Debug.WriteLine($"f[2] = {f.Data[2]} Expected {T.CreateTruncating(9.0)}");
        }

        [TestMethod]
        public void AddHalf() => Add<Half>();
        [TestMethod]
        public void AddFloat() => Add<float>();
        [TestMethod]
        public void AddDouble() => Add<double>();
        [TestMethod]
        public void AddDecimal() => Add<decimal>();

        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
        //[TestMethod]
        //public void AddByte() => Add<byte>();
        // The binary operator Add is not defined for the types 'System.Byte' and 'System.Byte'.
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
        public void AddBigInteger() => Add<BigInteger>();
    }
}