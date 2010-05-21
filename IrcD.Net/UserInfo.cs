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
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace IrcD
{

    public class UserInfo : InfoBase
    {
        public UserInfo(IrcDaemon ircDaemon, Socket socket, string host, bool isAcceptSocket, bool passAccepted)
            : base(ircDaemon)
        {
            this.host = host;
            this.isAcceptSocket = isAcceptSocket;
            this.passAccepted = passAccepted;
            this.socket = socket;
        }

        private Socket socket;

        internal Socket Socket
        {
            get
            {
                return socket;
            }
        }

        private bool isAcceptSocket;

        public bool IsAcceptSocket
        {
            get
            {
                return isAcceptSocket;
            }
        }

        private bool passAccepted;

        public bool PassAccepted
        {
            get
            {
                return passAccepted;
            }
            internal set
            {
                passAccepted = value;
            }
        }

        private bool registered = false;

        public bool Registered
        {
            get
            {
                return registered;
            }
            internal set
            {
                registered = value;
            }
        }

        private bool isService = false;

        public bool IsService
        {
            get
            {
                return isService;
            }
            set
            {
                isService = value;
            }
        }

        private string nick = null;

        public string Nick
        {
            get
            {
                return nick;
            }
            internal set
            {
                nick = value;
            }
        }

        private string realname = null;

        public string Realname
        {
            get
            {
                return realname;
            }
            internal set
            {
                realname = value;
            }
        }



        private string user = null;

        public string User
        {
            get
            {
                return user;
            }
            internal set
            {
                user = value;
            }
        }

        private string host;

        public string Host
        {
            get
            {
                return host;
            }
        }

        public string Usermask
        {
            get
            {
                return nick + "!" + user + "@" + host;
            }
        }

        public string Prefix
        {
            get
            {
                return ":" + Usermask;
            }
        }

        private string awayMsg = null;

        public string AwayMsg
        {
            get
            {
                return awayMsg;
            }
            set
            {
                awayMsg = value;
            }
        }

        private DateTime lastAction = DateTime.Now;

        public DateTime LastAction
        {
            get
            {
                return lastAction;
            }
            set
            {
                lastAction = value;
            }
        }


        public string ModeString
        {
            get
            {
                return "TODO";
            }
        }

        private readonly List<UserPerChannelInfo> channels = new List<UserPerChannelInfo>();

        public List<UserPerChannelInfo> Channels
        {
            get { return channels; }
        }

        public override string ToString()
        {
            return "TODO";
        }

        public override void WriteLine(StringBuilder line)
        {
            line.Append(IrcDaemon.ServerCrLf);

            socket.Send(Encoding.UTF8.GetBytes(line.ToString()));
        }
    }
}
