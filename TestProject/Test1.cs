using SharpGrad.Activation;
using SharpGrad.DifEngine;
using System.Diagnostics;

namespace TestProject
{
    [TestClass]
    public sealed class Test1
    {

        public static Random Rnd = new();
        [TestMethod]
        public void Add()
        {
            Variable<float> a = new(1.5f, "a");
            Variable<float> b = new(2.0f, "b");
            var c = a + b;
            var cFunc = c.ForwardLambda;
            cFunc();
            Debug.Assert(c.Data == 3.5f);

            a.Data = 2.0f;
            b.Data = 3.0f;
            cFunc();
            Debug.Assert(c.Data == 5.0f);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                var bData = (float)Rnd.NextDouble();
                b.Data = bData;
                cFunc();
                Debug.Assert(c.Data == aData + bData);
            }
        }

        [TestMethod]
        public void Sub()
        {
            Variable<float> a = new(1.5f, "a");
            Variable<float> b = new(2.0f, "b");
            var c = a - b;
            var cFunc = c.ForwardLambda;
            cFunc();
            Debug.Assert(c.Data == -0.5f);

            a.Data = 2.0f;
            b.Data = 3.0f;
            cFunc();
            Debug.Assert(c.Data == -1.0f);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                var bData = (float)Rnd.NextDouble();
                b.Data = bData;
                cFunc();
                Debug.Assert(c.Data == aData - bData);
            }
        }

        [TestMethod]
        public void Mul()
        {
            Variable<float> a = new(1.5f, "a");
            Variable<float> b = new(2.0f, "b");
            var c = a * b;
            var cFunc = c.ForwardLambda;
            cFunc();
            Debug.Assert(c.Data == 3.0f);

            a.Data = 2.0f;
            b.Data = 3.0f;
            cFunc();
            Debug.Assert(c.Data == 6.0f);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                var bData = (float)Rnd.NextDouble();
                b.Data = bData;
                cFunc();
                Debug.Assert(c.Data == aData * bData);
            }
        }

        [TestMethod]
        public void Div()
        {
            Variable<float> a = new(1.5f, "a");
            Variable<float> b = new(2.0f, "b");
            var c = a / b;
            var cFunc = c.ForwardLambda;
            cFunc();
            Debug.Assert(c.Data == 0.75f);

            a.Data = 2.0f;
            b.Data = 3.0f;
            cFunc();
            Debug.Assert(c.Data == 2.0f / 3.0f);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                var bData = (float)Rnd.NextDouble();
                b.Data = bData;
                cFunc();
                Debug.Assert(c.Data == aData / bData);
            }
        }

        [TestMethod]
        public void Pow()
        {
            Variable<float> a = new(1.5f, "a");
            Variable<float> b = new(2.0f, "b");
            var c = a.Pow(b);
            var cFunc = c.ForwardLambda;
            cFunc();
            Debug.Assert(c.Data == Math.Pow(1.5, 2.0));

            a.Data = 2.0f;
            b.Data = 3.0f;
            cFunc();
            Debug.Assert(c.Data == Math.Pow(2.0, 3.0));

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                var bData = (float)Rnd.NextDouble();
                b.Data = bData;
                cFunc();
                var r = (float)Math.Pow(aData, bData);
                Debug.Assert(c.Data == r);
            }
        }

        [TestMethod]
        public void Sigmoid()
        {
            Variable<float> a = new(1.5f, "a");
            var c = a.Sigmoid();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = 1.0f / (1.0f + (float)Math.Exp(-1.5f));
            Debug.Assert(c.Data == r);

            a.Data = 2.0f;
            cFunc();
            r = 1.0f / (1.0f + (float)Math.Exp(-2.0f));
            Debug.Assert(c.Data == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                cFunc();
                r = 1.0f / (1.0f + (float)Math.Exp(-aData));
                Debug.Assert(c.Data == r);
            }
        }

        [TestMethod]
        public void Tanh()
        {
            Variable<float> a = new(1.5f, "a");
            var c = a.Tanh();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = (float)Math.Tanh(1.5f);
            Debug.Assert(c.Data == r);

            a.Data = 2.0f;
            cFunc();
            r = (float)Math.Tanh(2.0f);
            Debug.Assert(c.Data == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                cFunc();
                r = (float)Math.Tanh(aData);
                Debug.Assert(c.Data == r);
            }
        }

        [TestMethod]
        public void ReLU()
        {
            Variable<float> a = new(1.5f, "a");
            var c = a.ReLU();
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = (float)Math.Max(0, 1.5f);
            Debug.Assert(c.Data == r);

            a.Data = -2.0f;
            cFunc();
            r = (float)Math.Max(0, -2.0f);
            Debug.Assert(c.Data == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                cFunc();
                r = (float)Math.Max(0, aData);
                Debug.Assert(c.Data == r);
            }
        }

        [TestMethod]
        public void LeakyReLU()
        {
            Variable<float> a = new(1.5f, "a");
            var c = a.LeakyReLU(0.1f);
            var cFunc = c.ForwardLambda;
            cFunc();
            var r = Math.Max(0.1f * 1.5f, 1.5f);
            Debug.Assert(c.Data == r);

            a.Data = -2.0f;
            cFunc();
            r = Math.Max(0.1f * -2.0f, -2.0f);
            Debug.Assert(c.Data == r);

            for (int i = 0; i < 10; i++)
            {
                var aData = (float)Rnd.NextDouble();
                a.Data = aData;
                cFunc();
                r = Math.Max(0.1f * aData, aData);
                Debug.Assert(c.Data == r);
            }
        }
    }
}
