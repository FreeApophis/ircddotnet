/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2017 Thomas Bruderer <apophis@apophis.ch>
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

using System;
using System.Collections.Generic;
using System.Net;
using IrcD.Core.Utils;

namespace IrcD.Core
{
    public class ServerOptions
    {

        internal ServerOptions(IrcMode ircMode)
        {
            IrcMode = ircMode;
            IrcCaseMapping = ircMode == IrcMode.Rfc1459 ? IrcCaseMapping.StrictRfc1459 : IrcCaseMapping.Rfc1459;
            ConnectionPasses = null;
        }

        public List<int> ServerPorts { get; set; } = new List<int> { 6667 };

        public string ServerPass { get; set; }

        public List<string> ConnectionPasses { get; }

        public Tuple<IPEndPoint, string> ServerConnection { get; set; }

        private string _serverName;

        public string ServerName
        {
            get
            {
                return string.IsNullOrEmpty(_serverName) ? "irc#d" : _serverName;
            }
            set
            {
                _serverName = value;
            }
        }

        public int MaxNickLength { get; set; } = 9;
        public int MaxLineLength { get; set; } = 510;
        public int MaxLanguages { get; set; } = 10;
        public int MaxSilence { get; set; } = 20;
        public int MaxChannelLength { get; set; } = 50;
        public int MaxTopicLength { get; set; } = 300;
        public int MaxKickLength { get; set; } = 300;
        public int MaxAwayLength { get; set; } = 300;

        public string MessageOfTheDay { get; set; }

        public string AdminLocation1 { get; set; }
        public string AdminLocation2 { get; set; }
        public string AdminEmail { get; set; }

        public string StandardPartMessage { get; set; } = "Leaving";
        public string StandardQuitMessage { get; set; } = "Quit";
        public string StandardKickMessage { get; set; } = "Kicked";
        public string StandardKillMessage { get; set; } = "Killed";

        public Dictionary<string, string> OLine { get; } = new Dictionary<string, string>();

        public List<OperHost> OperHosts { get; } = new List<OperHost>();

        /// <summary>
        /// Some clients have big problems with correct parsing of the RFC,
        /// this setting rearranges commands that even X-Chat and MIRC have
        /// a correct behaviour, however their implementation of the RFC
        /// especially their parsers are just stupid!
        /// </summary>
        public bool ClientCompatibilityMode { get; set; } = true;
        public IrcMode IrcMode { get; }
        public IrcCaseMapping IrcCaseMapping { get; set; }


        private string _networkName;

        public string NetworkName
        {
            get
            {
                return _networkName ?? "irc#d.NET";
            }
            set
            {
                _networkName = value;
            }
        }

    }
}
