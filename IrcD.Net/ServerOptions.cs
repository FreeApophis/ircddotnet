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
using IrcD.Modes;

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

        public IrcMode IrcMode { get; set; }
    }
}
