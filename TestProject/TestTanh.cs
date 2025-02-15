using SharpGrad;
using SharpGrad.Activation;
using SharpGrad.DifEngine;
using SharpGrad.Operator;
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
            var cFunc = c.Forward;
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

    [TestClass]
    public class TestSum
    {
        // Use SumValue<T> class from SharpGrad.Operator namespace
        public static void Sum<T>()
            where T : IBinaryFloatingPointIeee754<T>, IAdditionOperators<T, T, T>
        {
            Dimension dim1 = new("test", 3);
            Variable<T> a = new([
                T.CreateTruncating(1.0),
                T.CreateTruncating(2.0),
                T.CreateTruncating(3.0)],
                [dim1], "a");

            SumValue<T> sum = new([], "sum", a);
            sum.Forward();
            sum.Backward();
            Debug.Assert(sum.Data[0] == T.CreateTruncating(6.0));

            Dimension dim2 = new("test2", 2);
            Variable<T> b = new([
                T.CreateTruncating(1.0),
                T.CreateTruncating(2.0),
                T.CreateTruncating(3.0),
                T.CreateTruncating(4.0),
                T.CreateTruncating(5.0),
                T.CreateTruncating(6.0)],
                [dim1, dim2], "b");
            SumValue<T> sum2 = new([], "sum2", b);
            sum2.Forward();
            Debug.Assert(sum2.Data[0] == T.CreateTruncating(21.0));

            // Sum along the second dimension
            SumValue<T> sum3 = new([dim1], "sum3", b);
            sum3.Forward();
            Debug.Assert(sum3.Data[0] == T.CreateTruncating(1 + 2));
            Debug.Assert(sum3.Data[1] == T.CreateTruncating(3 + 4));
            Debug.Assert(sum3.Data[2] == T.CreateTruncating(5 + 6));


            // Sum along the first dimension
            SumValue<T> sum4 = new([dim2], "sum4", b);
            sum4.Forward();
            Debug.Assert(sum4.Data[0] == T.CreateTruncating(1 + 3 + 5));
            Debug.Assert(sum4.Data[1] == T.CreateTruncating(2 + 4 + 6));
        }

        [TestMethod]
        public void SumHalf() => Sum<Half>();
        [TestMethod]
        public void SumFloat() => Sum<float>();
        [TestMethod]
        public void SumDouble() => Sum<double>();
    }
}
