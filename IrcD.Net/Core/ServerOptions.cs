/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 *  
 * Copyright (c) 2009-2017, Thomas Bruderer, apophis@apophis.ch All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *   
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * * Neither the name of ArithmeticParser nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
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
