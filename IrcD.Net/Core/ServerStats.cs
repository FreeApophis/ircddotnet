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
using System.Linq;
using IrcD.Modes.UserModes;

namespace IrcD.Core
{
    public class ServerStats
    {
        readonly IrcDaemon _ircDaemon;

        public ServerStats(IrcDaemon ircDaemon)
        {
            _ircDaemon = ircDaemon;
        }

        public int ServerCount
        {
            get
            {
                return 1;
            }
        }

        public int ServiceCount
        {
            get
            {
                return _ircDaemon.Sockets.Count(s => s.Value.IsService);
            }
        }

        public int UserCount => _ircDaemon.Nicks.Count - ServiceCount;

        public int OperatorCount
        {
            get
            {
                return _ircDaemon.Sockets.Count(s => s.Value.Modes.Exist<ModeOperator>());
            }
        }

        public int LocalOperatorCount
        {
            get
            {
                return _ircDaemon.Sockets.Count(s => s.Value.Modes.Exist<ModeLocalOperator>());
            }
        }

        public int UnknowConnectionCount
        {
            get
            {
                return _ircDaemon.Sockets.Count(s => !s.Value.IsAcceptSocket);
            }
        }


        public int ChannelCount => _ircDaemon.Channels.Count;

        public int ClientCount => _ircDaemon.Nicks.Count;

        public TimeSpan Uptime => DateTime.Now - _ircDaemon.ServerCreated;
    }
}
