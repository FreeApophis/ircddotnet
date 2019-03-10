
using Xunit;

namespace IrcD.Test
{
    public class P10Numeric
    {
        [Fact]
        public void ServerNumeric()
        {
            var serverNumeric = new Core.Utils.P10Numeric(0);
            Assert.Equal("AA", serverNumeric.ToString());
            Assert.True(serverNumeric.IsServer);

            serverNumeric = new Core.Utils.P10Numeric(4095);
            Assert.Equal("]]", serverNumeric.ToString());
            Assert.True(serverNumeric.IsServer);
        }

        [Fact]
        public void ClientNumeric()
        {
            var clientNumeric = new Core.Utils.P10Numeric(2, 63);
            Assert.Equal("ACAA]", clientNumeric.ToString());
            Assert.False(clientNumeric.IsServer);

            clientNumeric = new Core.Utils.P10Numeric(4095, 262143);
            Assert.Equal("]]]]]", clientNumeric.ToString());
            Assert.False(clientNumeric.IsServer);
        }
    }
}
