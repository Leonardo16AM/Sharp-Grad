using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;

namespace TestProject
{
    [TestCategory("Basic")]
    [TestClass]
    public class BaseTest
    {
        [TestMethod]
        public void TestVariable()
        {
            var data = new float[] { 1, 2, 3, 4, 5 };
            Dimension[] shape = [new(nameof(shape), 5)];
            var var = new Variable<float>(data, shape, "var");
            Assert.AreEqual(5, var.Data.Length);
            Assert.AreEqual(1, var.Data[0]);
            Assert.AreEqual(2, var.Data[1]);
            Assert.AreEqual(3, var.Data[2]);
            Assert.AreEqual(4, var.Data[3]);
            Assert.AreEqual(5, var.Data[4]);
            Console.WriteLine($"{nameof(TestVariable)}({data.GetString()}) passed: {var.Data.GetString()}");
        }
        [TestMethod]
        public void TestConstant()
        {
            var data = new float[] { 1, 2, 3, 4, 5 };
            Dimension[] shape = [new(nameof(shape), 5)];
            var con = new Constant<float>(data, shape, "con");
            Assert.AreEqual(5, con.Data.Length);
            Assert.AreEqual(1, con.Data[0]);
            Assert.AreEqual(2, con.Data[1]);
            Assert.AreEqual(3, con.Data[2]);
            Assert.AreEqual(4, con.Data[3]);
            Assert.AreEqual(5, con.Data[4]);
            Console.WriteLine($"{nameof(TestConstant)}({data.GetString()}) passed: {con.Data.GetString()}");
        }
        [TestMethod]
        public void TestMSE()
        {
            Dimension batch = new(nameof(batch), 5);
            Dimension[] shape = [batch];
            Value<float> Y = new Variable<float>([1, 2, 3, 4, 5], shape, "Y");
            Value<float> Y_hat = new Variable<float>([1, 2, 3, 4, 5], shape, "Y_hat");
            var loss = Loss.MSE(Y, Y_hat, batch);
            loss.Forward();
            Assert.AreEqual(0, loss.Data[0]);
            Console.WriteLine($"{nameof(TestMSE)}({Y.Data.GetString()}, {Y_hat.Data.GetString()}) passed: {loss.Data.GetString()}");

            Y_hat = new Variable<float>([5, 4, 3, 2, 1], shape, "Y_hat");
            loss = Loss.MSE(Y, Y_hat, batch);
            loss.Forward();
            Assert.AreEqual(8, loss.Data[0]);
            Console.WriteLine($"{nameof(TestMSE)}({Y.Data.GetString()}, {Y_hat.Data.GetString()}) passed: {loss.Data.GetString()}");
        }
        [TestMethod]
        public void TestDimensionExtender()
        {
            Dimension[] dim = [new("X", 2), new("Y", 3), new("Z", 4)];
            Assert.AreEqual(24, dim.Size());
            Assert.IsFalse(dim.IsScalar());
            Assert.IsFalse(dim.IsVector());
            Console.WriteLine($"{nameof(TestDimensionExtender)}({dim.GetString()}) passed.");
        }
    }
}
