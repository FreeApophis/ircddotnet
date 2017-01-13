using System.Collections.Generic;
using IrcD.Tools;
using NUnit.Framework;

namespace IrcD.Test
{
    [TestFixture]
    public class Enumerable
    {
        [Test]
        public void Zip()
        {

            List<int> listA = new List<int> { 29, 15, 11, 28, 32 };
            List<int> listB = new List<int> { 9, 5, 1, 4, 9 };
            List<int> listC = new List<int> { 38, 20, 12, 32, 41 };

            foreach (bool x in listA.Zip(listB, (a, b) => (a + b)).Zip(listC, (ab, c) => (ab == c)))
            {
                Assert.IsTrue(x);
            }

        }

        [Test]
        public void Concatenate()
        {
            List<int> listA = new List<int> { 29, 15, 11, 28, 32 };

            string x = listA.Concatenate(", ");

            Assert.AreEqual(x, "29, 15, 11, 28, 32");
        }
    }
}
