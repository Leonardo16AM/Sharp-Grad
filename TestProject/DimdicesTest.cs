using SharpGrad;
using SharpGrad.DifEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestCategory("Basic")]
    [TestClass]
    public class DimdicesTest
    {
        [TestMethod]
        public void T()
        {
            Dimension X = new(3);
            Dimension Y = new(2);

            Variable<float> a = new([1, 2, 3, 4, 5, 6], [X, Y], "a");
            Variable<float> b = new([7, 8, 9], [X], "b");
            Variable<float> c = new([10, 11], [Y], "c");

            var d = a + b;
            d.ForwardLambda();
            Assert.AreEqual(8, d.Data[0]);
            Assert.AreEqual(9, d.Data[1]);
            Assert.AreEqual(11, d.Data[2]);
            Assert.AreEqual(12, d.Data[3]);
            Assert.AreEqual(14, d.Data[4]);
            Assert.AreEqual(15, d.Data[5]);

            var e = a + c;
            e.ForwardLambda();
            Assert.AreEqual(11, e.Data[0]);
            Assert.AreEqual(13, e.Data[1]);
            Assert.AreEqual(13, e.Data[2]);
            Assert.AreEqual(15, e.Data[3]);
            Assert.AreEqual(15, e.Data[4]);
            Assert.AreEqual(17, e.Data[5]);

            var f = b + c;
            f.ForwardLambda();
            Assert.AreEqual(17, f.Data[0]);
            Assert.AreEqual(18, f.Data[1]);
            Assert.AreEqual(19, f.Data[2]);
            Assert.AreEqual(18, f.Data[3]);
            Assert.AreEqual(19, f.Data[4]);
            Assert.AreEqual(20, f.Data[5]);

            var g = a + b + c;
            g.ForwardLambda();
            Assert.AreEqual(18, g.Data[0]);
            Assert.AreEqual(20, g.Data[1]);
            Assert.AreEqual(22, g.Data[2]);
            Assert.AreEqual(22, g.Data[3]);
            Assert.AreEqual(24, g.Data[4]);
            Assert.AreEqual(26, g.Data[5]);

            Console.WriteLine($"{nameof(T)} passed.");
        }

        [TestMethod]
        public void TestIndices()
        {
            Dimension X = new(3);
            Dimension Y = new(2);
            Dimension[] shape = [X, Y];

            Variable<float> a = new([1, 2, 3, 4, 5, 6], shape, "a");

            Dimdexer dimdexer = new(shape);
            foreach (Dimdices i in dimdexer)
            {
                int x = i[X];
                int y = i[Y];

                float ai = a[i];
                Assert.AreEqual((x * Y.Size) + y + 1, ai);
                Console.WriteLine($"a{i} = {ai}");
            }
        }
    }
}
