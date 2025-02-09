﻿using SharpGrad;
using SharpGrad.DifEngine;
using SharpGrad.NN;
using System.Linq.Expressions;

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
            var shape = new Dimension[] { new(5) };
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
            var shape = new Dimension[] { new(5) };
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
            Dimension[] shape = [new(5)];
            var Y = new Value<float>[] { new Variable<float>([1, 2, 3, 4, 5], shape, "Y") };
            var Y_hat = new Value<float>[] { new Variable<float>([1, 2, 3, 4, 5], shape, "Y_hat") };
            var loss = Loss.MSE(Y, Y_hat);
            Assert.AreEqual(0, loss.Data[0]);
            Console.WriteLine($"{nameof(TestMSE)}({Y[0].Data.GetString()}, {Y_hat[0].Data.GetString()}) passed: {loss.Data.GetString()}");
        }
        [TestMethod]
        public void TestDimensionExtender()
        {
            Dimension[] dim = [new(2), new(3), new(4)];
            Assert.AreEqual(24, dim.Size());
            Assert.IsFalse(dim.IsScalar());
            Assert.IsFalse(dim.IsVector());
            Console.WriteLine($"{nameof(TestDimensionExtender)}({dim.GetString()}) passed.");
        }
    }
}
