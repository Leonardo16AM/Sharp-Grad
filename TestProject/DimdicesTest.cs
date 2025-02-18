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
        public void AddMatrixAndVector()
        {
            Dimension X = new(nameof(X), 3);
            Dimension Y = new(nameof(Y), 2);

            Variable<float> a = new([1, 2, 3, 4, 5, 6], [X, Y], "a");
            Variable<float> b = new([7, 8, 9], [X], "b");
            Variable<float> c = new([10, 11], [Y], "c");

            var d = a + b;
            d.Forward();
            Assert.AreEqual(d.Data[0], a.Data[0] + b.Data[0]);
            Assert.AreEqual(d.Data[1], a.Data[1] + b.Data[0]);
            Assert.AreEqual(d.Data[2], a.Data[2] + b.Data[1]);
            Assert.AreEqual(d.Data[3], a.Data[3] + b.Data[1]);
            Assert.AreEqual(d.Data[4], a.Data[4] + b.Data[2]);
            Assert.AreEqual(d.Data[5], a.Data[5] + b.Data[2]);

            var e = a + c;
            e.Forward();
            Assert.AreEqual(e.Data[0], a.Data[0] + c.Data[0]);
            Assert.AreEqual(e.Data[1], a.Data[1] + c.Data[1]);
            Assert.AreEqual(e.Data[2], a.Data[2] + c.Data[0]);
            Assert.AreEqual(e.Data[3], a.Data[3] + c.Data[1]);
            Assert.AreEqual(e.Data[4], a.Data[4] + c.Data[0]);
            Assert.AreEqual(e.Data[5], a.Data[5] + c.Data[1]);

            var f = b + c;
            f.Forward();
            Assert.AreEqual(f.Data[0], b.Data[0] + c.Data[0]);
            Assert.AreEqual(f.Data[1], b.Data[0] + c.Data[1]);  
            Assert.AreEqual(f.Data[2], b.Data[1] + c.Data[0]);
            Assert.AreEqual(f.Data[3], b.Data[1] + c.Data[1]);
            Assert.AreEqual(f.Data[4], b.Data[2] + c.Data[0]);
            Assert.AreEqual(f.Data[5], b.Data[2] + c.Data[1]);

            var g = a + b + c;
            g.Forward();
            Assert.AreEqual(g.Data[0], a.Data[0] + b.Data[0] + c.Data[0]);
            Assert.AreEqual(g.Data[1], a.Data[1] + b.Data[0] + c.Data[1]);
            Assert.AreEqual(g.Data[2], a.Data[2] + b.Data[1] + c.Data[0]);
            Assert.AreEqual(g.Data[3], a.Data[3] + b.Data[1] + c.Data[1]);
            Assert.AreEqual(g.Data[4], a.Data[4] + b.Data[2] + c.Data[0]);
            Assert.AreEqual(g.Data[5], a.Data[5] + b.Data[2] + c.Data[1]);

            Console.WriteLine($"{nameof(AddMatrixAndVector)} passed.");
        }

        [TestMethod]
        public void TestIndices()
        {
            Dimension X = new(nameof(X), 3);
            Dimension Y = new(nameof(Y), 2);
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
