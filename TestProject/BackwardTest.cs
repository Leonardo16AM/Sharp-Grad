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

            // Gradient of A in A + B is 1
            Assert.AreEqual(1, A.Gradient[0]);
            Assert.AreEqual(1, A.Gradient[1]);
            Assert.AreEqual(1, A.Gradient[2]);

            // Gradient of B in A + B is 1
            Assert.AreEqual(1, B.Gradient[0]);
            Assert.AreEqual(1, B.Gradient[1]);
            Assert.AreEqual(1, B.Gradient[2]);

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

            // Gradient of A in A - B is 1
            Assert.AreEqual(1, A.Gradient[0]);
            Assert.AreEqual(1, A.Gradient[1]);
            Assert.AreEqual(1, A.Gradient[2]);

            // Gradient of B in A - B is -1
            Assert.AreEqual(-1, B.Gradient[0]);
            Assert.AreEqual(-1, B.Gradient[1]);
            Assert.AreEqual(-1, B.Gradient[2]);

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

            // Gradient of A in A * B is B
            Assert.AreEqual(4, A.Gradient[0]);
            Assert.AreEqual(5, A.Gradient[1]);
            Assert.AreEqual(6, A.Gradient[2]);

            // Gradient of B in A * B is A
            Assert.AreEqual(1, B.Gradient[0]);
            Assert.AreEqual(2, B.Gradient[1]);
            Assert.AreEqual(3, B.Gradient[2]);

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
            // Gradient of A in A / B is 1 / B
            Assert.AreEqual(1.0f / 4f, A.Gradient[0]);
            Assert.AreEqual(1.0f / 5f, A.Gradient[1]);
            Assert.AreEqual(1.0f / 6f, A.Gradient[2]);

            // Gradient of B in A / B is -A / B^2
            Assert.AreEqual(-1.0f / 16f, B.Gradient[0]);
            Assert.AreEqual(-2.0f / 25f, B.Gradient[1]);
            Assert.AreEqual(-3.0f / 36f, B.Gradient[2]);

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

            // Gradient of A in A ^ B is B * A ^ (B - 1)
            Assert.AreEqual(4 * MathF.Pow(1, 3), A.Gradient[0]);
            Assert.AreEqual(5 * MathF.Pow(2, 4), A.Gradient[1]);
            Assert.AreEqual(6 * MathF.Pow(3, 5), A.Gradient[2]);

            // Gradient of B in A ^ B is A ^ B * log(A)
            Assert.AreEqual(MathF.Pow(1, 4) * MathF.Log(1), B.Gradient[0]);
            Assert.AreEqual(MathF.Pow(2, 5) * MathF.Log(2), B.Gradient[1]);
            Assert.AreEqual(MathF.Pow(3, 6) * MathF.Log(3), B.Gradient[2]);

            Console.WriteLine($"{nameof(TestPow)}({A.Data.GetString()}, {B.Data.GetString()}) passed: {C.Data.GetString()}");
        }
    }
}
