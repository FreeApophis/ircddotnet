using System.Collections.Generic;

namespace IrcD
{
    class ServerOptions
    {
        private int serverPort = 6667;

        public int ServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
            }
        }

        private readonly List<char> channelPrefixes = new List<char> { '&', '#', '+', '!' };
        public List<char> ChannelPrefixes
        {
            get
            {
                return channelPrefixes;
            }
        }

        public string ServerPass { get; set; }

        public string ServerName { get; set; }


        private int nickLength = 9;

        public int NickLength
        {
            get
            {
                return nickLength;
            }
            set
            {
                nickLength = value;
            }
        }

        private List<string> motd = new List<string>();

        public List<string> MOTD
        {
            get
            {
                return motd;
            }
            set
            {
                motd = value;
            }
        }

        private readonly Dictionary<string, string> oLine = new Dictionary<string, string>();

        public Dictionary<string, string> OLine
        {
            get { return oLine; }
        }

        private bool clientCompatibilityMode = true;

        /// <summary>
        /// Some clients have big problems with correct parsing of the RFC, 
        /// this setting rearranges commands that even X-Chat and MIRC have 
        /// a correct behaviour, however their implementation of the RFC 
        /// especially their parsers are just stupid!
        /// </summary>
        public bool ClientCompatibilityMode
        {
            get
            {
                return clientCompatibilityMode;
            }
            set
            {
                clientCompatibilityMode = value;
            }
        }
    }
}
