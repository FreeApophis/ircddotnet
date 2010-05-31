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
using System.Linq;
using System.Net.Sockets;
using System.Text;
using IrcD.Modes;
using IrcD.Utils;

namespace IrcD
{

    public class UserInfo : InfoBase
    {
        public UserInfo(IrcDaemon ircDaemon, Socket socket, string host, bool isAcceptSocket, bool passAccepted)
            : base(ircDaemon)
        {
            IsService = false;
            Registered = false;
            PassAccepted = passAccepted;
            Host = host;

            this.isAcceptSocket = isAcceptSocket;
            this.socket = socket;
        }

        private readonly Socket socket;

        internal Socket Socket
        {
            get
            {
                return socket;
            }
        }

        private readonly bool isAcceptSocket;

        public bool IsAcceptSocket
        {
            get
            {
                return isAcceptSocket;
            }
        }

        public bool PassAccepted { get; internal set; }

        public bool Registered { get; internal set; }

        public bool IsService { get; set; }

        public string User { get; private set; }
        public string Nick { get; private set; }
        public string RealName { get; private set; }
        public string Host { get; private set; }


        public void InitNick(string nick)
        {
            Nick = nick;
        }

        public void InitUser(string user, string realname)
        {
            User = user;
            RealName = realname;
        }

        internal bool UserExists
        {
            get
            {
                return User != null;
            }
        }

        public bool NickExists
        {
            get
            {
                return Nick != null;
            }
        }

        public void Rename(string newNick)
        {
            foreach (var channel in Channels)
            {
                var channelInfo = channel.UserPerChannelInfos[Nick];
                channel.UserPerChannelInfos.Remove(Nick);
                channel.UserPerChannelInfos.Add(newNick, channelInfo);
            }

        }

        public string Usermask
        {
            get
            {
                return Nick + "!" + User + "@" + Host;
            }
        }

        public string Prefix
        {
            get
            {
                return ":" + Usermask;
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

        private DateTime lastPing = DateTime.Now;

        public DateTime LastPing
        {
            get
            {
                return lastPing;
            }
            set
            {
                lastPing = value;
            }
        }
        public string ModeString
        {
            get
            {
                return mode.ToString();
            }
        }

        private readonly List<UserPerChannelInfo> userPerChannelInfos = new List<UserPerChannelInfo>();

        public IEnumerable<UserPerChannelInfo> UserPerChannelInfos
        {
            get
            {
                return userPerChannelInfos;
            }
        }

        public IEnumerable<ChannelInfo> Channels
        {
            get
            {
                return userPerChannelInfos.Select(upci => upci.ChannelInfo);
            }
        }

        private readonly UserModeList mode = new UserModeList();

        public UserModeList Mode
        {
            get
            {
                return mode;
            }
        }


        public override string ToString()
        {
            return "TODO";
        }

        public override void WriteLine(StringBuilder line)
        {
            Logger.Log(line.ToString());

            line.Append(IrcDaemon.ServerCrLf);
            socket.Send(Encoding.UTF8.GetBytes(line.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick"></param>
        /// <returns></returns>
        public static bool ValidNick(string nick)
        {
            // TODO: implement nick check
            return true;
        }
    }
}
