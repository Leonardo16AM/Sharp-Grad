using SharpGrad;
using SharpGrad.DifEngine;

namespace TestProject
{
    [TestCategory("Backward")]
    [TestClass]
    public class BackwardTest
    {
        /// <summary>
        /// Test a composit function of ((A + B) * 2) / 3)
        /// Where A = [1, 2, 3] and B = [4, 5, 6].
        /// </summary>
        [TestMethod]
        public void TestCompisit3()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = (A + B);
            var C2 = C * 2;
            var C3 = C2 / 3;

            C3.Forward();
            C3.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of C3 in ((A + B) * 2) / 3 is 1

            // Gradient of C2 in ((A + B) * 2) / 3 is 2
            dimdexer.MoveNext(); Assert.AreEqual(1 / 3f, C2.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1 / 3f, C2.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1 / 3f, C2.GetGradient(dimdexer.Current));

            // Gradient of C in ((A + B) * 2) / 3 is 2 / 3
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, C.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, C.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, C.GetGradient(dimdexer.Current));

            // Gradient of A in ((A + B) * 2) / 3 is 2 / 3
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, A.GetGradient(dimdexer.Current));

            // Gradient of B in ((A + B) * 2) / 3 is 2 / 3
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f / 3, B.GetGradient(dimdexer.Current));
        }

        /// <summary>
        /// Test a composit function of (A + B) * 2)
        /// Where A = [1, 2, 3] and B = [4, 5, 6].
        /// </summary>
        [TestMethod]
        public void TestCompisit2()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = (A + B);
            var C2 = C * 2;
            C2.Forward();
            C2.Backward();
            Dimdexer dimdexer = new(shape);
            // Gradient of C2 in (A + B) * 2 is 1

            // Gradient of C in (A + B) * 2 is 2
            dimdexer.MoveNext(); Assert.AreEqual(2f, C.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, C.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, C.GetGradient(dimdexer.Current));
            // Gradient of A in (A + B) * 2 is 2
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
            // Gradient of B in (A + B) * 2 is 2
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(2f, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, B.GetGradient(dimdexer.Current));
        }

        /// <summary>
        /// Test a composit function of A * 2
        /// Where A = [1, 2, 3].
        /// </summary>
        [TestMethod]
        public void TestCompisit1()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var C = A * 2;
            C.Forward();
            C.Backward();
            Dimdexer dimdexer = new(shape);
            // Gradient of C in A * 2 is 1

            // Gradient of A in A * 2 is 2
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(2f, A.GetGradient(dimdexer.Current));
        }

        /// <summary>
        /// Test the addition operator A + B.
        /// Where A = [1, 2, 3] and B = [4, 5, 6].
        /// </summary>
        [TestMethod]
        public void TestAdd()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A + B;
            C.Forward();
            C.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A + B is 1
            dimdexer.MoveNext(); Assert.AreEqual(1, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1, A.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1, A.GetGradient(dimdexer.Current));

            // Gradient of B in A + B is 1
            dimdexer.Reset();
            dimdexer.MoveNext(); Assert.AreEqual(1, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1, B.GetGradient(dimdexer.Current));
            dimdexer.MoveNext(); Assert.AreEqual(1, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestAdd)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestSub()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A - B;
            C.Forward();
            C.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A - B is 1
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();

            // Gradient of B in A - B is -1
            Assert.AreEqual(-1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(-1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(-1, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestSub)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestMul()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A * B;
            C.Forward();
            C.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A * B is B
            dimdexer.MoveNext();
            Assert.AreEqual(4, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(5, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(6, A.GetGradient(dimdexer.Current));

            // Gradient of B in A * B is A
            dimdexer.Reset();
            dimdexer.MoveNext();
            Assert.AreEqual(1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(2, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(3, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestMul)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestDiv()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A / B;
            C.Forward();
            C.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A / B is 1 / B
            dimdexer.MoveNext();
            Assert.AreEqual(A.GetGradient(dimdexer.Current), 1.0f / B.Data[0]); dimdexer.MoveNext();
            Assert.AreEqual(A.GetGradient(dimdexer.Current), 1.0f / B.Data[1]); dimdexer.MoveNext();
            Assert.AreEqual(A.GetGradient(dimdexer.Current), 1.0f / B.Data[2]);

            // Gradient of B in A / B is -A / B^2
            dimdexer.Reset();
            dimdexer.MoveNext();
            Assert.AreEqual(B.GetGradient(dimdexer.Current), -A.Data[0] / (B.Data[0] * B.Data[0])); dimdexer.MoveNext();
            Assert.AreEqual(B.GetGradient(dimdexer.Current), -A.Data[1] / (B.Data[1] * B.Data[1])); dimdexer.MoveNext();
            Assert.AreEqual(B.GetGradient(dimdexer.Current), -A.Data[2] / (B.Data[2] * B.Data[2]));

            Console.WriteLine($"{nameof(TestDiv)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestPow()
        {
            Dimension[] shape = [new(nameof(shape), 3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A.Pow(B);
            C.Forward();
            C.Backward();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A ^ B is B * A ^ (B - 1)
            dimdexer.MoveNext();
            Assert.AreEqual(4 * MathF.Pow(1, 3), A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(5 * MathF.Pow(2, 4), A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(6 * MathF.Pow(3, 5), A.GetGradient(dimdexer.Current));

            // Gradient of B in A ^ B is A ^ B * log(A)
            dimdexer.Reset();
            dimdexer.MoveNext();
            Assert.AreEqual(MathF.Pow(1, 4) * MathF.Log(1), B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(MathF.Pow(2, 5) * MathF.Log(2), B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(MathF.Pow(3, 6) * MathF.Log(3), B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestPow)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }
    }
}
