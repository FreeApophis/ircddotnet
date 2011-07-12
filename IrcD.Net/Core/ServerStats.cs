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

using System;
using System.Linq;
using IrcD.Modes.UserModes;

namespace IrcD
{
    public class ServerStats
    {
        readonly IrcDaemon ircDaemon;

        public ServerStats(IrcDaemon ircDaemon)
        {
            this.ircDaemon = ircDaemon;
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
                return ircDaemon.Sockets.Count(s => s.Value.IsService);
            }
        }

        public int UserCount
        {
            get
            {
                return ircDaemon.Nicks.Count - ServiceCount;
            }
        }

        public int OperatorCount
        {
            get
            {
                return ircDaemon.Sockets.Count(s => s.Value.Modes.Exist<ModeOperator>());
            }
        }

        public int LocalOperatorCount
        {
            get
            {
                return ircDaemon.Sockets.Count(s => s.Value.Modes.Exist<ModeLocalOperator>());
            }
        }

        public int UnknowConnectionCount
        {
            get
            {
                return ircDaemon.Sockets.Count(s => !s.Value.IsAcceptSocket);
            }
        }


        public int ChannelCount
        {
            get
            {
                return ircDaemon.Channels.Count;
            }
        }

        public int ClientCount
        {
            get
            {
                return ircDaemon.Nicks.Count;
            }
        }

        public TimeSpan Uptime
        {
            get
            {
                return DateTime.Now - ircDaemon.ServerCreated;
            }
        }
    }
}
