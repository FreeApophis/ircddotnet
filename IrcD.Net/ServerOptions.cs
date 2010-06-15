/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2010 Thomas Bruderer <apophis@apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace IrcD
{
    public class ServerOptions
    {

        internal ServerOptions(IrcMode ircMode)
        {
            this.ircMode = ircMode;
            IrcCaseMapping = ircMode == IrcMode.Rfc1459 ? IrcCaseMapping.StrictRfc1459 : IrcCaseMapping.Rfc1459;
        }

        private int serverPort = 6667;

        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        public string ServerPass { get; set; }


        private string serverName;

        public string ServerName
        {
            get
            {
                return string.IsNullOrEmpty(serverName) ? "ircd.net" : serverName;
            }
            set
            {
                serverName = value;
            }
        }


        private int maxNickLength = 9;
        public int MaxNickLength
        {
            get { return maxNickLength; }
            set { maxNickLength = value; }
        }

        private int maxLineLength = 510;
        public int MaxLineLength
        {
            get { return maxLineLength; }
            set { maxLineLength = value; }
        }

        private int maxLanguages = 10;
        public int MaxLanguages
        {
            get { return maxLanguages; }
            set { maxLanguages = value; }
        }

        private int maxChannelLength = 50;
        public int MaxChannelLength
        {
            get { return maxChannelLength; }
            set { maxChannelLength = value; }
        }

        private int maxTopicLength = 300;
        public int MaxTopicLength
        {
            get { return maxTopicLength; }
            set { maxTopicLength = value; }
        }

        private int maxKickLength = 300;
        public int MaxKickLength
        {
            get { return maxKickLength; }
            set { maxKickLength = value; }
        }        
        
        private int maxAwayLength = 300;
        public int MaxAwayLength
        {
            get { return maxAwayLength; }
            set { maxAwayLength = value; }
        }


        private List<string> motd = new List<string>();

        public List<string> MOTD
        {
            get { return motd; }
            set { motd = value; }
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
            get { return clientCompatibilityMode; }
            set { clientCompatibilityMode = value; }
        }

        private readonly IrcMode ircMode;
        public IrcMode IrcMode
        {
            get { return ircMode; }
        }

        public IrcCaseMapping IrcCaseMapping { get; set; }


        private string standardPartMessage = "Leaving";

        public string StandardPartMessage
        {
            get { return standardPartMessage; }
            set { standardPartMessage = value; }
        }

        private string standardQuitMessage = "Quit";

        public string StandardQuitMessage
        {
            get { return standardQuitMessage; }
            set { standardQuitMessage = value; }
        }

        private string standardKickMessage = "Kicked";

        public string StandardKickMessage
        {
            get { return standardKickMessage; }
            set { standardKickMessage = value; }
        }

        private string networkName;

        public string NetworkName
        {
            get
            {
                return networkName ?? "apophis.NET";
            }
            set
            {
                networkName = value;
            }
        }


    }
}
