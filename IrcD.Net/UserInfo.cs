/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 19.03.2009
 * Zeit: 15:18
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
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
