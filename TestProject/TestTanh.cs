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

            SumValue<T> sum = VMath.Sum(a, dim1);
            sum.Forward();
            sum.Backward();
            Debug.Assert(sum.Data[0] == T.CreateTruncating(6.0));
            Debug.WriteLine($"Test sum of [{string.Join(", ", a.Data)}] passed. Result: {sum.Data[0]}");

            Dimension dim2 = new("test2", 2);
            Variable<T> b = new([
                T.CreateTruncating(1.0),
                T.CreateTruncating(2.0),
                T.CreateTruncating(3.0),
                T.CreateTruncating(4.0),
                T.CreateTruncating(5.0),
                T.CreateTruncating(6.0)],
                [dim1, dim2], "b");
            SumValue<T> sum2 = VMath.Sum(b, dim1, dim2);
            sum2.Forward();
            sum2.Backward();
            Debug.Assert(sum2.Data[0] == T.CreateTruncating(21.0));
            Debug.WriteLine($"Test sum of [{string.Join(", ", b.Data)}] passed. Result: {sum2.Data[0]}");

            // Sum along the second dimension
            SumValue<T> sum3 = VMath.Sum(b, dim2);
            sum3.Forward();
            sum3.Backward();
            Debug.Assert(sum3.Data[0] == T.CreateTruncating(1 + 2));
            Debug.Assert(sum3.Data[1] == T.CreateTruncating(3 + 4));
            Debug.Assert(sum3.Data[2] == T.CreateTruncating(5 + 6));
            Debug.WriteLine($"Test sum of [{string.Join(", ", b.Data)}] along dim2 passed. Result: [{string.Join(", ", sum3.Data)}]");

            SumValue<T> sum3bis = VMath.Sum(sum3, dim1);
            sum3bis.Forward();
            sum3bis.Backward();
            Debug.Assert(sum3bis.Data[0] == T.CreateTruncating(1 + 2 + 3 + 4 + 5 + 6));
            Debug.WriteLine($"Test sum of [{string.Join(", ", sum3.Data)}] along dim1 passed. Result: [{string.Join(", ", sum3bis.Data)}]");

            // Sum along the first dimension
            SumValue<T> sum4 = VMath.Sum(b, dim1);
            sum4.Forward();
            sum4.Backward();
            Debug.Assert(sum4.Data[0] == T.CreateTruncating(1 + 3 + 5));
            Debug.Assert(sum4.Data[1] == T.CreateTruncating(2 + 4 + 6));
            Debug.WriteLine($"Test sum of [{string.Join(", ", b.Data)}] along dim1 passed. Result: [{string.Join(", ", sum4.Data)}]");

            SumValue<T> sum4bis = VMath.Sum(sum4, dim2);
            sum4bis.Forward();
            sum4bis.Backward();
            Debug.Assert(sum4bis.Data[0] == T.CreateTruncating(1 + 3 + 5 + 2 + 4 + 6));
            Debug.WriteLine($"Test sum of [{string.Join(", ", sum4.Data)}] along dim2 passed. Result: [{string.Join(", ", sum4bis.Data)}]");
        }

        [TestMethod]
        public void SumHalf() => Sum<Half>();
        [TestMethod]
        public void SumFloat() => Sum<float>();
        [TestMethod]
        public void SumDouble() => Sum<double>();
    }
}
