/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009 Thomas Bruderer <apophis@apophis.ch>
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
using System.Net.Sockets;

namespace IrcD
{
    
    class UserInfo {
        
        public UserInfo(IrcDaemon daemon, Socket socket, string host, bool isAcceptSocket, bool passAccepted) {
            this.daemon = daemon;
            this.host = host;
            this.isAcceptSocket = isAcceptSocket;
            this.passAccepted = passAccepted;
            this.socket = socket;
        }
        
        private IrcDaemon daemon;
        
        private Socket socket;
        
        internal Socket Socket {
            get {
                return socket;
            }
        }
        
        private bool isAcceptSocket;
        
        public bool IsAcceptSocket {
            get {
                return isAcceptSocket;
            }
        }
        
        private bool passAccepted;
        
        public bool PassAccepted {
            get {
                return passAccepted;
            }
            internal set {
                passAccepted = value;
            }
        }
        
        private bool registered = false;
        
        public bool Registered {
            get {
                return registered;
            }
            internal set {
                registered = value;
            }
        }
        
        private bool isService = false;
        
        public bool IsService {
            get { 
                return isService; 
            }
            set { 
                isService = value; 
            }
        }
        
        private string nick = null;
        
        public string Nick {
            get {
                return nick;
            }
            internal set {
                nick = value;
            }
        }
        
        private string realname = null;
        
        public string Realname {
            get {
                return realname;
            }
            internal set {
                realname = value;
            }
        }
        
        

        private string user = null;
        
        public string User {
            get {
                return user;
            }
            internal set {
                user = value;
            }
        }

        private string host;
        
        public string Host {
            get {
                return host;
            }
        }
        
        public string Usermask {
            get {
                return nick + "!" + user + "@" + host;
            }
        }
        
        public string Prefix {
            get {
                return ":" + Usermask;
            }
        }

        private string awayMsg = null;
        
        public string AwayMsg {
            get {
                return awayMsg;
            }
            set {
                awayMsg = value;
            }
        }
        
        private DateTime lastAction = DateTime.Now;
        
        public DateTime LastAction {
            get { 
                return lastAction; 
            }
            set {
                lastAction = value;
            }
        }
        
        public bool Mode_i { get; internal set; }
        public bool Mode_w { get; internal set; }
        public bool Mode_o { get; internal set; }
        public bool Mode_O { get; internal set; }
        public bool Mode_r { get; internal set; }
        public bool Mode_s { get; internal set; }
        
        public string ModeString {
            get {
                return "+" + ((string.IsNullOrEmpty(awayMsg))?"":"a") + ((Mode_i)?"i":"") + ((Mode_w)?"w":"") + ((Mode_o)?"o":"") + ((Mode_O)?"O":"") + ((Mode_r)?"r":"") + ((Mode_s)?"s":"");
            }
        }
        
        private List<ChannelInfo> channels = new List<ChannelInfo>();
        
        public List<ChannelInfo> Channels {
            get { return channels; }
        }
        
        public override string ToString()
        {
            return Usermask + " (" + Realname + ") " + ((Mode_i)?"+i":"-i") + ((Mode_w)?"+w":"-w") + ((Mode_o)?"+o":"-o") + ((Mode_O)?"+O":"-O") + ((Mode_i)?"+r":"-r");
        }
        
        
    }
}
