using System.Collections.Generic;
using IrcD.Tools;
using Xunit;

namespace IrcD.Test
{
    public class Enumerable
    {


        [Fact]
        public void Concatenate()
        {
            List<int> listA = new List<int> { 29, 15, 11, 28, 32 };

            string x = listA.Concatenate(", ");

            Assert.Equal("29, 15, 11, 28, 32", x);
        }
    }
}
