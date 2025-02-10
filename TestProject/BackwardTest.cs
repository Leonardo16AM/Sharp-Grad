using SharpGrad;
using SharpGrad.DifEngine;

namespace TestProject
{
    [TestCategory("Backward")]
    [TestClass]
    public class BackwardTest
    {
        [TestMethod]
        public void TestAdd()
        {
            Dimension[] shape = [new(3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A + B;
            C.ForwardLambda();
            C.BackwardLambda();

            Dimdexer dimdexer = new(shape);
            dimdexer.MoveNext();
            // Gradient of A in A + B is 1
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();

            // Gradient of B in A + B is 1
            Assert.AreEqual(1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestAdd)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestSub()
        {
            Dimension[] shape = [new(3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A - B;
            C.ForwardLambda();
            C.BackwardLambda();

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
            Dimension[] shape = [new(3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A * B;
            C.ForwardLambda();
            C.BackwardLambda();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A * B is B
            Assert.AreEqual(4, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(5, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(6, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();

            // Gradient of B in A * B is A
            Assert.AreEqual(1, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(2, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(3, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestMul)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestDiv()
        {
            Dimension[] shape = [new(3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A / B;
            C.ForwardLambda();
            C.BackwardLambda();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A / B is 1 / B
            Assert.AreEqual(1.0f / 4f, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1.0f / 5f, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(1.0f / 6f, A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();

            // Gradient of B in A / B is -A / B^2
            Assert.AreEqual(-1.0f / 16f, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(-2.0f / 25f, B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(-3.0f / 36f, B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestDiv)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }

        [TestMethod]
        public void TestPow()
        {
            Dimension[] shape = [new(3)];
            var A = new Variable<float>([1, 2, 3], shape, "A");
            var B = new Variable<float>([4, 5, 6], shape, "B");
            var C = A.Pow(B);
            C.ForwardLambda();
            C.BackwardLambda();

            Dimdexer dimdexer = new(shape);
            // Gradient of A in A ^ B is B * A ^ (B - 1)
            Assert.AreEqual(4 * MathF.Pow(1, 3), A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(5 * MathF.Pow(2, 4), A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(6 * MathF.Pow(3, 5), A.GetGradient(dimdexer.Current)); dimdexer.MoveNext();

            // Gradient of B in A ^ B is A ^ B * log(A)
            Assert.AreEqual(MathF.Pow(1, 4) * MathF.Log(1), B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(MathF.Pow(2, 5) * MathF.Log(2), B.GetGradient(dimdexer.Current)); dimdexer.MoveNext();
            Assert.AreEqual(MathF.Pow(3, 6) * MathF.Log(3), B.GetGradient(dimdexer.Current));

            Console.WriteLine($"{nameof(TestPow)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }
    }
}
