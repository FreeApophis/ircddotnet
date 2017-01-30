using NUnit.Framework;

namespace IrcD.Test
{
    [TestFixture]
    public class P10Numeric
    {
        [Test]
        public void ServerNumeric()
        {
            var serverNumeric = new Core.Utils.P10Numeric(0);
            Assert.AreEqual("AA", serverNumeric.ToString());
            Assert.IsTrue(serverNumeric.IsServer);

            serverNumeric = new Core.Utils.P10Numeric(4095);
            Assert.AreEqual("]]", serverNumeric.ToString());
            Assert.IsTrue(serverNumeric.IsServer);
        }

        [Test]
        public void ClientNumeric()
        {
            var clientNumeric = new Core.Utils.P10Numeric(2, 63);
            Assert.AreEqual("ACAA]", clientNumeric.ToString());
            Assert.IsFalse(clientNumeric.IsServer);

            clientNumeric = new Core.Utils.P10Numeric(4095, 262143);
            Assert.AreEqual("]]]]]", clientNumeric.ToString());
            Assert.IsFalse(clientNumeric.IsServer);
        }
    }
}
