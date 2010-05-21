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
using System.Net;
using System.Net.Sockets;
using System.Text;
using IrcD.Modes;
using IrcD.Modes.ChannelModes;
using IrcD.Modes.ChannelRanks;
using IrcD.Modes.UserModes;
using IrcD.Utils;

namespace IrcD
{

    public class IrcDaemon
    {

        const int MaxBufferSize = 2048;
        const int MaxServerLineLength = 800;

        const string NumericFormat = "{0:000}";
        public const string ServerCrLf = "\r\n";
        const char PrefixCharacter = ':';

        private delegate void CommandDelegate(UserInfo info, List<string> args);

        private readonly Dictionary<Socket, UserInfo> sockets = new Dictionary<Socket, UserInfo>();
        private readonly Dictionary<string, ChannelInfo> channels = new Dictionary<string, ChannelInfo>();
        private readonly Dictionary<string, Socket> nicks = new Dictionary<string, Socket>();
        private readonly Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>(StringComparer.CurrentCultureIgnoreCase);

        // Supported
        private readonly ModeList<ChannelRank> supportedRanks = new ModeList<ChannelRank>();
        private readonly ModeList<ChannelMode> supportedChannelModes = new ModeList<ChannelMode>();
        private readonly ModeList<UserMode> supportedUserModes = new ModeList<UserMode>();

        private bool connected = true;
        private byte[] buffer = new byte[MaxBufferSize];
        private EndPoint ep = new IPEndPoint(0, 0);
        private EndPoint localEp;

        private StringBuilder commandSB = new StringBuilder(MaxServerLineLength);

        private readonly ServerOptions options = new ServerOptions();
        public ServerOptions Options
        {
            get
            {
                return options;
            }
        }

        public string ServerPrefix
        {
            get
            {
                return PrefixCharacter + Options.ServerName;
            }
        }

        private readonly DateTime serverCreated;



        public IrcDaemon()
        {

            serverCreated = DateTime.Now;

            AddRfcCommands();
            // RFC 2812 - optional
            AddRfcOptCommands();
            // Nonstandard IRC Commands
            AddNonRfcCommands();
            // Add Modes
            AddModes();
        }

        private void AddModes()
        {
            supportedRanks.Add(new ModeVoice());
            supportedRanks.Add(new ModeOp());

            supportedChannelModes.Add(new ModeBan());
            supportedChannelModes.Add(new ModeBanException());
            supportedChannelModes.Add(new ModeColorless());
            supportedChannelModes.Add(new ModeKey());
            supportedChannelModes.Add(new ModeLimit());
            supportedChannelModes.Add(new ModeModerated());
            supportedChannelModes.Add(new ModeNoExternal());
            supportedChannelModes.Add(new ModePrivate());

            supportedUserModes.Add(new ModeInvisible());
            supportedUserModes.Add(new ModeRestricted());
        }

        void AddNonRfcCommands()
        {
            commands.Add("KNOCK", knockDelegate);
        }


        void AddRfcOptCommands()
        {
            commands.Add("AWAY", AwayDelegate);
            commands.Add("REHASH", rehashDelegate);
            commands.Add("DIE", dieDelegate);
            commands.Add("RESTART", restartDelegate);
            commands.Add("SUMMON", summonDelegate);
            commands.Add("USERS", usersDelegate);
            commands.Add("WALLOPS", wallopsDelegate);
            commands.Add("USERHOST", userhostDelegate);
            commands.Add("ISON", IsonDelegate);
        }


        void AddRfcCommands()
        {
            commands.Add("PASS", PassDelegate);
            commands.Add("NICK", NickDelegate);
            commands.Add("USER", UserDelegate);
            commands.Add("OPER", OperDelegate);
            commands.Add("MODE", ModeDelegate); /* both user and channel - the RFC Splits the description */
            commands.Add("SERVICE", ServiceDelegate);
            commands.Add("QUIT", QuitDelegate);
            commands.Add("SQUIT", SquitDelegate);
            commands.Add("JOIN", JoinDelegate);
            commands.Add("PART", PartDelegate);
            commands.Add("TOPIC", TopicDelegate);
            commands.Add("NAMES", NamesDelegate);
            commands.Add("LIST", ListDelegate);
            commands.Add("INVITE", InviteDelegate);
            commands.Add("KICK", KickDelegate);
            commands.Add("PRIVMSG", PrivmsgDelegate);
            commands.Add("NOTICE", NoticeDelegate);
            commands.Add("MOTD", MOTDDelegate);
            commands.Add("LUSERS", LusersDelegate);
            commands.Add("VERSION", VersionDelegate);
            commands.Add("STATS", StatsDelegate);
            commands.Add("LINKS", LinksDelegate);
            commands.Add("TIME", TimeDelegate);
            commands.Add("CONNECT", ConnectDelegate);
            commands.Add("TRACE", TraceDelegate);
            commands.Add("ADMIN", AdminDelegate);
            commands.Add("INFO", InfoDelegate);
            commands.Add("SERVLIST", ServlistDelegate);
            commands.Add("SQUERY", SqueryDelegate);
            commands.Add("WHO", WhoDelegate);
            commands.Add("WHOIS", WhoisDelegate);
            commands.Add("WHOWAS", WhowasDelegate);
            commands.Add("KILL", KillDelegate);
            commands.Add("PING", PingDelegate);
            commands.Add("PONG", PongDelegate);
            commands.Add("ERROR", ErrorDelegate);
        }


        public void MainLoop()
        {

            localEp = new IPEndPoint(IPAddress.Any, Options.ServerPort);
            var connectSocket = new Socket(localEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            connectSocket.Bind(localEp);
            connectSocket.Listen(20);

            sockets.Add(connectSocket, new UserInfo(this, connectSocket, "TODO:Server", true, true));

            while (connected)
            {
                var activeSockets = new List<Socket>(sockets.Keys);

                Socket.Select(activeSockets, null, null, 2000000);

                foreach (Socket s in activeSockets)
                {
                    try
                    {
                        if (sockets[s].IsAcceptSocket)
                        {
                            Socket temp = s.Accept();
                            sockets.Add(temp, new UserInfo(this, temp, ((IPEndPoint)temp.RemoteEndPoint).Address.ToString(), false, String.IsNullOrEmpty(Options.ServerPass)));
                            Logger.Log("Client connected!");
                        }
                        else
                        {
                            try
                            {
                                buffer.Initialize();
                                int numBytes = s.ReceiveFrom(buffer, ref ep);
                                foreach (string line in Encoding.UTF8.GetString(buffer, 0, numBytes).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    Parser(line, s, sockets[s]);
                                }
                            }
                            catch (SocketException e)
                            {
                                Logger.Log("ERROR: " + e.Message + "(CODE:" + e.ErrorCode + ")");
                                QuitDelegate(sockets[s], new List<string> { "Socket reset by peer" });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Unknown ERROR: " + e.Message);
                        Logger.Log("Trace: " + e.StackTrace);
                    }
                }
            }
        }

        public void Parser(string line, Socket sock, UserInfo info)
        {

            string prefix = null;
            string command = null;
            var replycode = ReplyCode.Null;
            var args = new List<string>();

            try
            {
                int i = 0;
                /* This runs in the mainloop :: parser needs to return fast
                 * -> nothing which could block it may be called inside Parser
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                if (line[0] == PrefixCharacter)
                {
                    /* we have a prefix */
                    while (line[++i] != ' ') { }
                    prefix = line.Substring(1, i - 1);
                }
                else
                {
                    prefix = info.Usermask;
                }

                int commandStart = i;
                /*command might be numeric (xxx) or command */
                if (char.IsDigit(line[i + 1]) && char.IsDigit(line[i + 2]) && char.IsDigit(line[i + 3]))
                {
                    replycode = (ReplyCode)int.Parse(line.Substring(i + 1, 3));
                    i += 4;
                }
                else
                {
                    while ((i < (line.Length - 1)) && line[++i] != ' ') { }
                    if (line.Length - 1 == i) { ++i; }
                    command = line.Substring(commandStart, i - commandStart);
                }

                ++i;
                int paramStart = i;
                while (i < line.Length)
                {
                    if (line[i] == ' ' && i != paramStart)
                    {
                        args.Add(line.Substring(paramStart, i - paramStart));
                        paramStart = i + 1;
                    }
                    if (line[i] == PrefixCharacter)
                    {
                        if (paramStart != i)
                        {
                            args.Add(line.Substring(paramStart, i - paramStart));
                        }
                        args.Add(line.Substring(i + 1));
                        break;
                    }
                    ++i;
                }

                if (i == line.Length)
                {
                    args.Add(line.Substring(paramStart));
                }

            }
            catch (IndexOutOfRangeException)
            {
                // invalid message
            }

            if (command == null)
                return;

            CommandDelegate commandHandler;
            if (commands.TryGetValue(command, out commandHandler))
            {
                commandHandler.Invoke(info, args);
            }
            else
            {
#if DEBUG
                Logger.Log("Command " + command + "is not yet implemented");
#endif

                if (info.Registered)
                {
                    // we only inform the client about invalid commands if he is already successfully registered
                    // we dont want to make "wrong protocol ping-pong"
                    SendUnknownCommand(info, command);
                }

            }

#if DEBUG
            Logger.Log(line);
            var parsedLine = new StringBuilder();
            parsedLine.Append("[" + info.Usermask + "]-[" + command + "]");

            foreach (string arg in args)
            {
                parsedLine.Append("-<" + arg + ">");
            }

            Logger.Log(parsedLine.ToString());

#endif
        }

        #region Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick"></param>
        /// <returns></returns>
        private bool ValidNick(string nick)
        {
            // TODO: implement nick check
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private bool ValidChannel(string channel)
        {
            // TODO: implement channel check
            if (!string.IsNullOrEmpty(channel) && Options.ChannelPrefixes.Contains(channel[0]))
            {
                return true;
            }
            return false;
        }

        private string[] GetSubArgument(string arg)
        {
            return arg.Split(new[] { ',' });
        }

        private Dictionary<char, bool> ParseUserMode(string umode)
        {
            var changemodes = new Dictionary<char, bool>();
            bool plus = true; // if + or - is ommited at beginning we treat it as +
            foreach (char c in umode)
            {
                switch (c)
                {
                    case '+': plus = true;
                        break;
                    case '-': plus = false;
                        break;
                    default: changemodes.Add(c, plus);
                        break;
                }
            }
            return changemodes;
        }

        private readonly List<char> modeWithParams = new List<char> { 'b', 'e', 'h', 'I', 'k', 'l', 'o', 'O', 'v' };

        private class ModeElement
        {
            public ModeElement(char mode, bool? plus, string param)
            {
                this.mode = mode;
                this.plus = plus;
                this.param = param;
            }

            private char mode;

            public char Mode
            {
                get { return mode; }
            }

            private bool? plus;

            public bool? Plus
            {
                get { return plus; }
            }

            private string param;

            public string Param
            {
                get { return param; }
            }

        }

        private List<ModeElement> ParseChannelModes(List<string> cmode)
        {
            var changemodes = new List<ModeElement>();
            bool? plus;
            int arg = 1;
            int paramsNeeded;
            while (arg < cmode.Count)
            {
                plus = null; paramsNeeded = 0;
                foreach (char c in cmode[arg])
                {
                    if (c == '+')
                    {
                        plus = true;
                    }
                    else if (c == '-')
                    {
                        plus = false;
                    }
                    else
                    {
                        if (modeWithParams.Contains(c))
                        {
                            paramsNeeded++;
                            if (plus == null)
                            {
                                changemodes.Add(new ModeElement(c, plus, null));
                            }
                            else
                            {
                                try
                                {
                                    changemodes.Add(new ModeElement(c, plus, cmode[arg + paramsNeeded]));
                                }
                                catch (ArgumentOutOfRangeException) { }
                            }
                        }
                        else
                        {
                            changemodes.Add(new ModeElement(c, plus, null));
                        }

                    }
                }
                arg = arg + paramsNeeded + 1;
            }

            return changemodes;
        }

        /// <summary>
        /// Send the messages after registering complete
        /// </summary>
        /// <param name="info"></param>
        private void RegisterComplete(UserInfo info)
        {
            SendWelcome(info);
            SendYourHost(info);
            SendCreated(info);
            SendMyInfo(info);
            SendISupport(info);
            if (Options.MOTD.Count == 0)
            {
                SendNoMotd(info);
            }
            else
            {
                SendMotdStart(info);
                SendMotd(info);
                SendMotdEnd(info);
            }
        }

        /// <summary>
        /// Check if an IRC Operatur status can be granted upon user and pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private bool IsIrcOp(string user, string pass)
        {
            string realpass;
            if (Options.OLine.TryGetValue(user, out realpass))
            {
                if (pass == realpass)
                    return true;
            }
            return false;
        }
        #endregion

        #region Numeric Server Replies
        /// <summary>
        /// Reply Code 001
        /// </summary>
        /// <param name="info"></param>
        private void SendWelcome(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Welcome);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Welcome to the Internet Relay Network ");
            commandSB.Append(info.Usermask);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 002
        /// </summary>
        /// <param name="info"></param>
        private void SendYourHost(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.YourHost);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Your host is ");
            commandSB.Append(Options.ServerName);
            commandSB.Append(", running version ");
            commandSB.Append(System.Reflection.Assembly.GetExecutingAssembly().FullName);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 003
        /// </summary>
        /// <param name="info"></param>
        private void SendCreated(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Created);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :This server was created ");
            commandSB.Append(serverCreated);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 004
        /// </summary>
        /// <param name="info"></param>
        private void SendMyInfo(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.MyInfo);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :");
            commandSB.Append(Options.ServerName);
            commandSB.Append(" ");
            commandSB.Append("ircD.Net.<version> <available user modes> <available channel modes>");
            commandSB.Append(" ");
            commandSB.Append(supportedUserModes.ToString());
            commandSB.Append(" ");
            commandSB.Append(supportedRanks.ToString() + supportedChannelModes);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 005
        /// </summary>
        /// <param name="info"></param>
        private void SendISupport(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ISupport);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            // TODO: features supported by server
            commandSB.Append(" :are supported by this server");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 005 / 010
        /// </summary>
        /// <param name="info"></param>
        private void SendBounce(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Bounce);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            // TODO: bounce to which server
            commandSB.Append(" :Try server <server name>, port <port number>");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 221
        /// </summary>
        /// <param name="info"></param>
        private void SendUserModeIs(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.UserModeIs);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(info.ModeString);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 301
        /// </summary>
        /// <param name="info"></param>
        /// <param name="awayUser"></param>
        private void SendAwayMsg(UserInfo info, UserInfo awayUser)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Away);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(awayUser.Nick);
            commandSB.Append(" :");
            commandSB.Append(awayUser.AwayMsg);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 303
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nickList"></param>
        private void SendIsOn(UserInfo info, IEnumerable<string> nickList)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.IsOn);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :");
            foreach (string nick in nickList)
            {
                commandSB.Append(nick + " ");
            }
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 305
        /// </summary>
        /// <param name="info"></param>
        private void SendUnAway(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.UnAway);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You are no longer marked as being away");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 306
        /// </summary>
        /// <param name="info"></param>
        private void SendNowAway(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.NowAway);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You have been marked as being away");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }


        /// <summary>
        /// Reply 311
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendWhoIsUser(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsUser);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.User);
            commandSB.Append(" ");
            commandSB.Append(who.Host);
            commandSB.Append(" * :");
            commandSB.Append(who.Realname);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 312
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendWhoIsServer(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsServer);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" ");
            commandSB.Append(Options.ServerName); // TODO: when doing multiple IRC Server
            commandSB.Append(" :");
            commandSB.Append("IRC#Daemon Server Info"); // TODO: ServerInfo?
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 313
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendWhoIsOperator(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsOperator);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" :is an IRC operator");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 317
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendWhoIsIdle(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsIdle);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" ");
            commandSB.Append((DateTime.Now - who.LastAction).TotalSeconds);
            commandSB.Append(" :seconds idle");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 318
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendEndOfWhoIs(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.EndOfWhoIs);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" ");
            commandSB.Append(" :End of WHOIS list");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 319
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        private void SendWhoIsChannels(UserInfo info, UserInfo who)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsChannels);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(who.Nick);
            commandSB.Append(" :");
            foreach (var chan in who.Channels.Select(c => c.ChannelInfo))
            {
                commandSB.Append("");   // TODO: nickprefix (is in UPCI)
                commandSB.Append(chan.Name);
                commandSB.Append(" ");
                // TODO: Split at max length
            }
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 322
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        private void SendListItem(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.List);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" ");
            commandSB.Append(chan.Users.Count);
            commandSB.Append(" :");
            commandSB.Append(chan.Topic);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 323
        /// </summary>
        /// <param name="info"></param>
        private void SendListEnd(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ListEnd);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :End of LIST");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 331
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        private void SendNoTopicReply(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.NoTopic);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :No topic is set");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 332
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        private void SendTopicReply(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Topic);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :" + chan.Topic);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 353
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        private void SendNamesReply(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.NamesReply);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" = ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :");
            foreach (UserPerChannelInfo upci in chan.Users.Values)
            {
                commandSB.Append(upci.Modes.NickPrefix);
                commandSB.Append(upci.UserInfo.Nick);
                commandSB.Append(" ");
                // TODO: Split at max length
            }
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 366
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        private void SendEndOfNamesReply(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.EndOfNames);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :End of NAMES list");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 372
        /// </summary>
        /// <param name="info"></param>
        private void SendMotd(UserInfo info)
        {
            foreach (string motdLine in Options.MOTD)
            {
                commandSB.Length = 0;
                commandSB.Append(ServerPrefix);
                commandSB.Append(" ");
                commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Motd);
                commandSB.Append(" ");
                commandSB.Append(info.Nick);
                commandSB.Append(" :- ");
                commandSB.Append(motdLine);
                commandSB.Append(ServerCrLf);
                info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
            }
        }

        /// <summary>
        /// Reply Code 375
        /// </summary>
        /// <param name="info"></param>
        private void SendMotdStart(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.MotdStart);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :- ");
            commandSB.Append(Options.ServerName);
            commandSB.Append(" Message of the day -");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }


        /// <summary>
        /// Reply Code 376
        /// </summary>
        /// <param name="info"></param>
        private void SendMotdEnd(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.EndOfMotd);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :End of MOTD command");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 381
        /// </summary>
        /// <param name="info"></param>
        private void SendYouAreOper(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.YouAreOper);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You are now an IRC operator");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }
        /// <summary>
        /// Reply Code 383
        /// </summary>
        /// <param name="info"></param>
        private void SendYouAreService(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.YouAreService);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You are service ");
            commandSB.Append(info.Nick);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 391
        /// </summary>
        /// <param name="info"></param>
        private void SendTimeReply(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.Time);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(ServerPrefix);
            commandSB.Append(" :");
            commandSB.Append(DateTime.Now.ToString());
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }
        #endregion

        #region Numeric Error Replies
        /// <summary>
        /// Reply Code 401
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        protected void SendNoSuchNick(UserInfo info, string nick)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchNickname);
            commandSB.Append(" ");
            commandSB.Append(nick);
            commandSB.Append(" :No such nick/channel");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 402
        /// </summary>
        /// <param name="info"></param>
        /// <param name="server"></param>
        protected void SendNoSuchServer(UserInfo info, string server)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchServer);
            commandSB.Append(" ");
            commandSB.Append(server);
            commandSB.Append(" :No such server");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 403
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        protected void SendNoSuchChannel(UserInfo info, string channel)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchChannel);
            commandSB.Append(" ");
            commandSB.Append(channel);
            commandSB.Append(" :No such channel");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 404
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        protected void SendCannotSendToChannel(UserInfo info, string channel)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorCannotSendToChannel);
            commandSB.Append(" ");
            commandSB.Append(channel);
            commandSB.Append(" :Cannot send to channel");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 405
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        protected void Send(UserInfo info, string channel)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorTooManyChannels);
            commandSB.Append(" ");
            commandSB.Append(channel);
            commandSB.Append(" :You have joined too many channels");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 411
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        protected void SendNoRecipient(UserInfo info, string command)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoRecipient);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :No recipient given (");
            commandSB.Append(command);
            commandSB.Append(")");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 412
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoTextToSend(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoTextToSend);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :No text to send");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 421
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        protected void SendUnknownCommand(UserInfo info, string command)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUnknownCommand);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(command);
            commandSB.Append(" :Unknown command");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 422
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoMotd(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoMotd);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :MOTD File is missing");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 431
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoNicknameGiven(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoNicknameGiven);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :No nickname given");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 432
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        protected void SendErroneousNickname(UserInfo info, string nick)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorErroneusNickname);
            commandSB.Append(" ");
            commandSB.Append(nick);
            commandSB.Append(" :Erroneous nickname");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 433
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        protected void SendNicknameInUse(UserInfo info, string nick)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNicknameInUse);
            commandSB.Append(" ");
            commandSB.Append(nick);
            commandSB.Append(" :Nickname is already in use");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 442
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        protected void SendNotOnChannel(UserInfo info, string channel)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNotOnChannel);
            commandSB.Append(" ");
            commandSB.Append(channel);
            commandSB.Append(" :You're not on that channel");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 445
        /// </summary>
        /// <param name="info"></param>
        protected void SendSummonDisabled(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorSummonDisabled);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :SUMMON has been disabled");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 446
        /// </summary>
        /// <param name="info"></param>
        protected void SendUsersDisabled(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUsersDisabled);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :USERS has been disabled");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 451
        /// </summary>
        /// <param name="info"></param>
        protected void SendNotRegistered(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNotRegistered);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You have not registered");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Numeric Reply 461
        /// </summary>
        /// <param name="info"></param>
        protected void SendNeedMoreParams(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNeedMoreParams);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Not enough parameters");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Numeric Reply 462
        /// </summary>
        /// <param name="info"></param>
        protected void SendAlreadyRegistered(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorAlreadyRegistered);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Unauthorized command (already registered)");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Numeric Reply 463
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoPermissionForHost(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoPermissionForHost);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Your host isn't among the privileged");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 464
        /// </summary>
        /// <param name="info"></param>
        protected void SendPasswordMismatch(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorPasswordMismatch);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Password incorrect");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 465
        /// </summary>
        /// <param name="info"></param>
        protected void SendYouAreBannedCreep(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorYouAreBannedCreep);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You are banned from this server");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 466
        /// </summary>
        /// <param name="info"></param>
        protected void SendYouWillBeBanned(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorYouWillBeBanned);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You will be banned from this server");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 471
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendChannelIsFull(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorChannelIsFull);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Cannot join channel (+l)");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 472
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mode"></param>
        protected void SendUnknownMode(UserInfo info, ChannelInfo chan, char mode)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUnknownMode);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(mode);
            commandSB.Append(" :is unknown mode char to me for ");
            commandSB.Append(chan.Name);
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 473
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendInviteOnlyChannel(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorInviteOnlyChannel);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Cannot join channel (+i)");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }
        /// <summary>
        /// Reply Code 474
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendBannedFromChannel(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBannedFromChannel);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Cannot join channel (+b)");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 475
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendBadChannelKey(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBadChannelKey);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Cannot join channel (+k)");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 476
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendBadChannelMask(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBadChannelMask);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Bad Channel Mask");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 477
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendNoChannelModes(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoChannelModes);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :Channel doesn't support modes");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 478
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mode"></param>
        protected void SendBanListFull(UserInfo info, ChannelInfo chan, char mode)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBanListFull);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" ");
            commandSB.Append(mode);
            commandSB.Append(" :Channel list is full");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 481
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoPrivileges(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoPrivileges);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Permission Denied- You're not an IRC operator");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 482
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        protected void SendChannelOpPrivilegesNeeded(UserInfo info, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorChannelOpPrivilegesNeeded);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :You're not channel operator");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 483
        /// </summary>
        /// <param name="info"></param>
        protected void SendCannotKillServer(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorCannotKillServer);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You can't kill a server!");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 484
        /// </summary>
        /// <param name="info"></param>
        protected void SendRestricted(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorRestricted);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Your connection is restricted!");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 485
        /// </summary>
        /// <param name="info"></param>
        protected void SendUniqueOpPrivilegesNeeded(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUniqueOpPrivilegesNeeded);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :You're not the original channel operator");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply 491
        /// </summary>
        /// <param name="info"></param>
        protected void SendNoOperHost(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoOperHost);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :No O-lines for your host");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 501
        /// </summary>
        /// <param name="info"></param>
        protected void SendUserModeUnknownFlag(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUserModeUnknownFlag);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Unknown MODE flag");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        /// <summary>
        /// Reply Code 502
        /// </summary>
        /// <param name="info"></param>
        protected void SendUsersDoNotMatch(UserInfo info)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" ");
            commandSB.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUsersDoNotMatch);
            commandSB.Append(" ");
            commandSB.Append(info.Nick);
            commandSB.Append(" :Cannot change mode for other users");
            commandSB.Append(ServerCrLf);
            info.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }
        #endregion

        #region Server Messages
        internal void SendNick(UserInfo sender, InfoBase receiver, string newnick)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" NICK ");
            commandSB.Append(newnick);
            receiver.WriteLine(commandSB);
        }

        internal void SendJoin(UserInfo sender, UserInfo receiver, ChannelInfo chan)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" JOIN ");
            commandSB.Append(chan.Name);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendPart(UserInfo sender, UserInfo receiver, ChannelInfo chan, string message)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" PART ");
            commandSB.Append(chan.Name);
            commandSB.Append(" :");
            commandSB.Append(message);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendTopic(UserInfo sender, UserInfo receiver, string chan, string newtopic)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" TOPIC ");
            commandSB.Append(chan);
            commandSB.Append(" :");
            commandSB.Append(newtopic);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendMode(UserInfo sender, UserInfo receiver, string target, string modestring)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" MODE ");
            commandSB.Append(target);
            commandSB.Append(" ");
            commandSB.Append(modestring);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }


        internal void SendPrivMsg(UserInfo sender, UserInfo receiver, string target, string message)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" PRIVMSG ");
            commandSB.Append(target);
            commandSB.Append(" :");
            commandSB.Append(message);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendNotice(UserInfo sender, UserInfo receiver, string target, string message)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" NOTICE ");
            commandSB.Append(target);
            commandSB.Append(" :");
            commandSB.Append(message);
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendQuit(UserInfo sender, InfoBase receiver, string message)
        {
            commandSB.Length = 0;
            commandSB.Append(sender.Prefix);
            commandSB.Append(" QUIT :");
            commandSB.Append(message);

            receiver.WriteLine(commandSB);
        }

        internal void SendPing(UserInfo receiver)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" PING ");
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }

        internal void SendPong(UserInfo receiver)
        {
            commandSB.Length = 0;
            commandSB.Append(ServerPrefix);
            commandSB.Append(" PONG ");
            commandSB.Append(ServerCrLf);
            receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        }
        #endregion

        #region Command Delegates - RF2812 - required
        internal void PassDelegate(UserInfo info, List<string> args)
        {
            if (info.PassAccepted)
            {
                SendAlreadyRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNeedMoreParams(info);
                return;
            }
            if (args[0] == Options.ServerPass)
            {
                info.PassAccepted = true;
                return;
            }
            SendPasswordMismatch(info);
        }

        internal void NickDelegate(UserInfo info, List<string> args)
        {
            if (!info.PassAccepted)
            {
                SendPasswordMismatch(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNoNicknameGiven(info);
                return;
            }
            if (nicks.ContainsKey(args[0]))
            {
                SendNicknameInUse(info, args[0]);
                return;
            }
            if (!ValidNick(args[0]))
            {
                SendErroneousNickname(info, args[0]);
                return;
            }
            // NICK command valid after this point
            if (info.Nick != null)
            {
                nicks.Remove(info.Nick);
            }

            nicks.Add(args[0], info.Socket);

            foreach (var channelInfo in info.Channels.Select(channel => channel.ChannelInfo))
            {
                SendNick(info, channelInfo, args[0]);
            }

            info.Nick = args[0];

            if ((!info.Registered) && (info.User != null))
            {
                info.Registered = true;
                RegisterComplete(info);
            }
        }

        internal void UserDelegate(UserInfo info, List<string> args)
        {
            if (!info.PassAccepted)
            {
                SendPasswordMismatch(info);
                return;
            }
            if (info.User != null)
            {
                SendAlreadyRegistered(info);
                return;
            }
            if (args.Count < 4)
            {
                SendNeedMoreParams(info);
                return;
            }

            int flags;
            int.TryParse(args[1], out flags);

            //TODO: new Modes
            //info.Mode_i = ((flags & 8) > 0);
            //info.Mode_w = ((flags & 4) > 0);

            info.User = args[0];

            info.Realname = args[3];

            if (info.Nick != null)
            {
                info.Registered = true;
                // *** send welcome ***
                RegisterComplete(info);
            }
        }

        internal void OperDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 2)
            {
                SendNeedMoreParams(info);
                return;
            }

            // TODO: deny certain hosts OPER status
            if (false)
            {
                SendNoOperHost(info);
                return;
            }

            if (IsIrcOp(args[0], args[1]))
            {
                // TODO: new modes
                //info.Mode_o = true;
                //info.Mode_O = true;
                SendYouAreOper(info);
            }
            else
            {
                SendPasswordMismatch(info);
            }
        }


        internal void ModeDelegate(UserInfo info, List<string> args)
        {
            // TODO: new modes
            //if (!info.Registered)
            //{
            //    SendNotRegistered(info);
            //    return;
            //}
            //if (args.Count == 0)
            //{
            //    SendNeedMoreParams(info);
            //    return;
            //}

            //if (ValidChannel(args[0]))
            //{
            //    if (!channels.ContainsKey(args[0]))
            //    {
            //        // TODO: Send Chan does not exist;
            //        return;
            //    }
            //    ChannelInfo chan = channels[args[0]];
            //    if (args.Count == 1)
            //    {
            //        // TODO: which modes should we send?
            //    }
            //    else
            //    {
            //        string reply = "";
            //        Dictionary<string, List<string>> compatibilityCache = null;
            //        foreach (ModeElement cmode in ParseChannelModes(args))
            //        {
            //            switch (cmode.Mode)
            //            {
            //                case 'a':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_a = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'b':
            //                    if (cmode.Plus != null)
            //                    {
            //                        if (cmode.Plus.Value)
            //                        {
            //                            chan.Mode_b.Add(cmode.Param);
            //                        }
            //                        else
            //                        {
            //                            chan.Mode_b.Remove(cmode.Param);
            //                        }
            //                    }
            //                    break;
            //                case 'e':
            //                    if (cmode.Plus != null)
            //                    {
            //                        if (cmode.Plus.Value)
            //                        {
            //                            chan.Mode_e.Add(cmode.Param);
            //                        }
            //                        else
            //                        {
            //                            chan.Mode_e.Remove(cmode.Param);
            //                        }
            //                    }
            //                    break;
            //                case 'h':
            //                    if (cmode.Param == null || cmode.Plus == null)
            //                        throw new ArgumentNullException();
            //                    if (chan.User.ContainsKey(cmode.Param))
            //                    {
            //                        chan.User[cmode.Param].Mode_h = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'i':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_i = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'I':
            //                    if (cmode.Plus != null)
            //                    {
            //                        if (cmode.Plus.Value)
            //                        {
            //                            chan.Mode_I.Add(cmode.Param);
            //                        }
            //                        else
            //                        {
            //                            chan.Mode_I.Remove(cmode.Param);
            //                        }
            //                    }
            //                    break;
            //                case 'k':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_k = cmode.Plus.Value ? cmode.Param : null;
            //                    }
            //                    break;
            //                case 'l':
            //                    if (cmode.Param == null)
            //                        throw new ArgumentNullException();
            //                    if (cmode.Plus != null)
            //                    {
            //                        if (cmode.Plus.Value)
            //                        {
            //                            chan.Mode_l = int.Parse(cmode.Param);
            //                        }
            //                        else
            //                        {
            //                            chan.Mode_l = -1;
            //                        }
            //                    }
            //                    break;
            //                case 'm':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_m = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'n':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_n = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'o':
            //                    if (cmode.Param == null || cmode.Plus == null)
            //                        throw new ArgumentNullException();
            //                    if (chan.User.ContainsKey(cmode.Param))
            //                    {
            //                        chan.User[cmode.Param].Mode_o = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'O':
            //                    break;
            //                case 'p':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_p = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'q':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_q = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'r':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_r = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 's':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_s = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 't':
            //                    if (cmode.Plus != null)
            //                    {
            //                        chan.Mode_t = cmode.Plus.Value;
            //                    }
            //                    break;
            //                case 'v':
            //                    if (cmode.Param == null || cmode.Plus == null)
            //                        throw new ArgumentNullException();
            //                    if (chan.User.ContainsKey(cmode.Param))
            //                    {
            //                        chan.User[cmode.Param].Mode_v = cmode.Plus.Value;
            //                    }
            //                    break;
            //                default:
            //                    SendUnknownMode(info, chan, cmode.Mode);
            //                    continue;
            //            }

            //            if (Options.ClientCompatibilityMode)
            //            {
            //                compatibilityCache = compatibilityCache ?? new Dictionary<string, List<string>>();
            //                string key = ((cmode.Plus.HasValue) ? cmode.Plus.Value ? "+" : "-" : "") + cmode.Mode;
            //                if (compatibilityCache.ContainsKey(key))
            //                {
            //                    compatibilityCache[key].Add(cmode.Param ?? "");
            //                }
            //                else
            //                {
            //                    compatibilityCache.Add(key, new List<string> { cmode.Param ?? "" });
            //                }
            //            }
            //            else
            //            {
            //                //TODO: this is very convinient, but mIRC and xchat cannot parse this (RTF RFC)
            //                if (cmode.Plus == null)
            //                {
            //                    reply += cmode.Mode + " ";
            //                }
            //                else if (cmode.Plus.Value)
            //                {
            //                    reply += "+" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
            //                }
            //                else
            //                {
            //                    reply += "-" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
            //                }
            //            }
            //        }
            //        if (Options.ClientCompatibilityMode && compatibilityCache != null)
            //        {
            //            foreach (var modes in compatibilityCache)
            //            {

            //                if (modes.Key[0] == '+' || modes.Key[0] == '-')
            //                {
            //                    reply += modes.Key[0];
            //                    reply += new string(modes.Key[1], modes.Value.Count);
            //                    foreach (var param in modes.Value)
            //                    {
            //                        if (param.Length > 0)
            //                        {
            //                            reply += " " + param;
            //                        }
            //                    }
            //                    reply += " ";
            //                }
            //                else
            //                {
            //                    reply += new string(modes.Key[0], modes.Value.Count) + " ";
            //                }
            //            }
            //        }

            //        if (reply.Length > 0)
            //        {
            //            foreach (UserPerChannelInfo upci in chan.User.Values)
            //            {
            //                SendMode(info, upci.Info, chan.Name, reply);
            //            }
            //        }

            //    }
            //}
            //else if (args[0] == info.Nick)
            //{
            //    if (args.Count == 1)
            //    {
            //        SendUserModeIs(info);
            //    }
            //    else
            //    {
            //        string reply = "";
            //        foreach (KeyValuePair<char, bool> umode in ParseUserMode(args[1]))
            //        {
            //            switch (umode.Key)
            //            {
            //                case 'i':
            //                    info.Mode_i = umode.Value;
            //                    break;
            //                case 'O':
            //                    if (!umode.Value)
            //                    {
            //                        info.Mode_O = false;
            //                    }
            //                    else
            //                    {
            //                        continue;
            //                    }
            //                    break;
            //                case 'o':
            //                    if (!umode.Value)
            //                    {
            //                        info.Mode_o = false;
            //                    }
            //                    else
            //                    {
            //                        continue;
            //                    }
            //                    break;
            //                case 'r':
            //                    if (umode.Value)
            //                    {
            //                        info.Mode_r = true;
            //                    }
            //                    else
            //                    {
            //                        continue;
            //                    }
            //                    break;
            //                case 's':
            //                    info.Mode_s = umode.Value;
            //                    break;
            //                case 'w':
            //                    info.Mode_w = umode.Value;
            //                    break;
            //                default:
            //                    SendUserModeUnknownFlag(info);
            //                    continue;
            //            }
            //            reply += ((umode.Value) ? "+" : "-") + umode.Key;

            //        }
            //        if (reply.Length > 0)
            //        {
            //            SendMode(info, info, info.Nick, reply);
            //        }
            //    }
            //}
            //else
            //{
            //    SendUsersDoNotMatch(info);
            //}
        }

        internal void ServiceDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (info.Registered)
            {
                SendAlreadyRegistered(info);
                return;
            }
            if (args.Count < 6)
            {
                SendNeedMoreParams(info);
                return;
            }
            if (!ValidNick(args[0]))
            {
                SendErroneousNickname(info, args[0]);
                return;
            }
            if (nicks.ContainsKey(args[0]))
            {
                SendNicknameInUse(info, args[0]);
                return;
            }

            info.Nick = args[0];
            nicks.Add(info.Nick, info.Socket);

            info.User = "service";
            info.IsService = true;
            info.Registered = true;

            SendYouAreService(info);
            SendYourHost(info);
            SendMyInfo(info);
        }

        internal void QuitDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            string message = (args.Count > 0) ? args[0] : "";

            foreach (ChannelInfo channelInfo in info.Channels.Select(upci => upci.ChannelInfo))
            {
                SendQuit(info, channelInfo, message);
            }

            foreach (ChannelInfo chaninfo in info.Channels.Select(upci => upci.ChannelInfo))
            {
                chaninfo.Users.Remove(info.Nick);
            }

            sockets.Remove(info.Socket);
            nicks.Remove(info.Nick);
            info.Socket.Close();

        }

        internal void SquitDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        internal void JoinDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNeedMoreParams(info);
                return;
            }
            if (args[0] == "0")
            {
                var partargs = new List<string>();
                // this is a part all channels, this is plainly stupid to handle PARTS in a join message.
                // we won't handle that, we give it to the part handler! YO! why not defining a /part * instead of /join 0
                commandSB.Length = 0; bool first = true;
                foreach (ChannelInfo ci in info.Channels.Select(upci => upci.ChannelInfo))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        commandSB.Append(",");
                    }
                    commandSB.Append(ci.Name);
                }
                partargs.Add(commandSB.ToString());
                partargs.Add("Left all channels");
                PartDelegate(info, partargs);
                return;
            }

            IEnumerable<string> keys;
            if (args.Count > 1)
            {
                keys = GetSubArgument(args[1]);
            }
            else
            {
                keys = new List<string>();
            }

            foreach (string ch in GetSubArgument(args[0]))
            {
                ChannelInfo chan;
                if (channels.ContainsKey(ch))
                {
                    chan = channels[ch];
                    // TODO: new modes
                    // Check for (+l)
                    //if ((chan.Mode_l != -1) && (chan.Mode_l <= chan.User.Count))
                    //{
                    //    SendChannelIsFull(info, chan);
                    //    return;
                    //}

                    // Check for (+k)
                    //if (!string.IsNullOrEmpty(chan.Mode_k))
                    //{
                    //    bool j = false;
                    //    foreach (string key in keys)
                    //    {
                    //        if (key == chan.Mode_k)
                    //        {
                    //            j = true;
                    //        }
                    //    }
                    //    if (!j)
                    //    {
                    //        SendBadChannelKey(info, chan);
                    //        return;
                    //    }
                    //}

                    // Check for (+i)
                    //if (chan.Mode_i)
                    //{
                    //    // TODO: implement invite
                    //    SendInviteOnlyChannel(info, chan);
                    //}

                    // Check for (+b) (TODO)
                    if (false)
                    {
                        SendBannedFromChannel(info, chan);
                    }
                }
                else
                {
                    chan = new ChannelInfo(ch, this);
                    channels.Add(chan.Name, chan);
                }

                var chanuser = new UserPerChannelInfo(info, chan);
                chan.Users.Add(info.Nick, chanuser);
                info.Channels.Add(chanuser);

                foreach (UserPerChannelInfo upci in chan.Users.Values)
                {
                    SendJoin(info, upci.UserInfo, chan);
                }


                if (string.IsNullOrEmpty(chan.Topic))
                {
                    SendNoTopicReply(info, chan);
                }
                else
                {
                    SendTopicReply(info, chan);
                }
                SendNamesReply(chanuser.UserInfo, chan);
                SendEndOfNamesReply(info, chan);
            }

        }

        internal void PartDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNeedMoreParams(info);
                return;
            }
            string message = (args.Count > 1) ? args[1] : "";

            foreach (string ch in GetSubArgument(args[0]))
            {
                if (channels.ContainsKey(ch))
                {
                    ChannelInfo chan = channels[ch];
                    //if (info.Channels.Contains(chan))
                    //{
                    //    foreach (UserPerChannelInfo upci in chan.User.Values)
                    //    {
                    //        SendPart(info, upci.Info, chan, message);
                    //    }
                    //    chan.User.Remove(info.Nick);
                    //    info.Channels.Remove(chan);
                    //}
                    //else
                    //{
                    //    SendNotOnChannel(info, ch);
                    //    continue;
                    //}
                }
                else
                {
                    SendNoSuchChannel(info, ch);
                    continue;
                }
            }
        }

        internal void TopicDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            switch (args.Count)
            {
                case 0:
                    SendNeedMoreParams(info);
                    break;
                case 1:
                    if (channels.ContainsKey(args[0]))
                    {
                        if (string.IsNullOrEmpty(channels[args[0]].Topic))
                        {
                            SendNoTopicReply(info, channels[args[0]]);
                        }
                        else
                        {
                            SendTopicReply(info, channels[args[0]]);
                        }
                    }
                    else
                    {
                        SendNoSuchChannel(info, args[0]);
                    }
                    break;
                case 2:
                    if (channels.ContainsKey(args[0]))
                    {
                        channels[args[0]].Topic = args[1];
                        foreach (UserPerChannelInfo upci in channels[args[0]].Users.Values)
                        {
                            SendTopic(info, upci.UserInfo, args[0], args[1]);
                        }
                    }
                    break;
                default:
                    // TODO: Protocol error too many params
                    break;

            }
        }

        internal void NamesDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            if (args.Count < 1)
            {
                // TODO: list all visible users
                return;
            }

            //TODO: taget parameter
            foreach (string ch in GetSubArgument(args[0]))
            {
                if (channels.ContainsKey(ch))
                {
                    SendNamesReply(info, channels[ch]);
                }
            }

        }

        private void ListDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            // TODO: special LIST commands / implemented is full list
            foreach (ChannelInfo ci in channels.Values)
            {
                SendListItem(info, ci);
            }
            SendListEnd(info);
        }

        private void InviteDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void KickDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
        }

        private void PrivmsgDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNoRecipient(info, "PRIVMSG");
                return;
            }
            if (args.Count < 2)
            {
                SendNoTextToSend(info);
                return;
            }

            // TODO: idle timer, which commands reset them?
            info.LastAction = DateTime.Now;

            if (ValidChannel(args[0]))
            {
                if (channels.ContainsKey(args[0]))
                {
                    ChannelInfo chan = channels[args[0]];
                    //if (chan.Mode_n && (!info.Channels.Contains(chan)) /*TODO: banned user cannot send even without Mode_n set*/)
                    //{
                    //    SendCannotSendToChannel(info, chan.Name);
                    //    return;
                    //}
                    //if (!chan.Mode_m || (chan.Mode_m && info.Channels.Contains(chan) &&
                    //                    (chan.User[info.Nick].Mode_v || chan.User[info.Nick].Mode_h || chan.User[info.Nick].Mode_o)))
                    //{
                    //    foreach (UserPerChannelInfo upci in chan.User.Values)
                    //    {
                    //        if (upci.Info.Nick != info.Nick)
                    //        {
                    //            SendPrivMsg(info, upci.Info, chan.Name, args[1]);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    SendCannotSendToChannel(info, chan.Name);
                    //}
                }
                else
                {
                    SendNoSuchChannel(info, args[0]);
                }
            }
            else if (ValidNick(args[0]))
            {
                if (nicks.ContainsKey(args[0]))
                {
                    UserInfo user = sockets[nicks[args[0]]];
                    if (user.AwayMsg != null)
                    {
                        SendAwayMsg(info, user);
                    }
                    SendPrivMsg(info, user, user.Nick, args[1]);
                }
                else
                {
                    SendNoSuchNick(info, args[0]);
                }
            }
            else
            {
                SendNoSuchNick(info, args[0]);
            }
        }

        private void NoticeDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNoRecipient(info, "NOTICE");
                return;
            }
            if (args.Count < 2)
            {
                SendNoTextToSend(info);
                return;
            }

            // TODO: idle timer, which commands reset them?
            info.LastAction = DateTime.Now;

            if (ValidChannel(args[0]))
            {
                if (channels.ContainsKey(args[0]))
                {
                    //ChannelInfo chan = channels[args[0]];
                    //if (chan.Mode_n && (!info.Channels.Contains(chan)) /*TODO: banned user cannot send even without Mode_n set*/)
                    //{
                    //    SendCannotSendToChannel(info, chan.Name);
                    //    return;
                    //}
                    //if (!chan.Mode_m || (chan.Mode_m && info.Channels.Contains(chan) &&
                    //                    (chan.User[info.Nick].Mode_v || chan.User[info.Nick].Mode_h || chan.User[info.Nick].Mode_o)))
                    //{
                    //    foreach (UserPerChannelInfo upci in chan.User.Values)
                    //    {
                    //        if (upci.Info.Nick != info.Nick)
                    //        {
                    //            SendNotice(info, upci.Info, chan.Name, args[1]);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    SendCannotSendToChannel(info, chan.Name);
                    //}
                }
                else
                {
                    SendNoSuchChannel(info, args[0]);
                }
            }
            else if (ValidNick(args[0]))
            {
                if (nicks.ContainsKey(args[0]))
                {
                    UserInfo user = sockets[nicks[args[0]]];
                    if (user.AwayMsg != null)
                    {
                        SendAwayMsg(info, user);
                    }
                    SendNotice(info, user, user.Nick, args[1]);
                }
                else
                {
                    SendNoSuchNick(info, args[0]);
                }
            }
            else
            {
                SendNoSuchNick(info, args[0]);
            }
        }

        private void MOTDDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            // TODO: parameter 1 parsing

            if (Options.MOTD.Count == 0)
            {
                SendNoMotd(info);
            }
            else
            {
                SendMotdStart(info);
                SendMotd(info);
                SendMotdEnd(info);
            }
        }

        private void LusersDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void VersionDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void StatsDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void LinksDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void TimeDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            //TODO: Parse Server Argument
            SendTimeReply(info);
        }

        private void ConnectDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void TraceDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void AdminDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void InfoDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void ServlistDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void SqueryDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void WhoDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void WhoisDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNeedMoreParams(info);
                return;
            }
            if (!nicks.ContainsKey(args[0]))
            {
                SendNoSuchNick(info, args[0]);
                return;
            }

            UserInfo user = sockets[nicks[args[0]]];
            SendWhoIsUser(info, user);
            if (user.Channels.Count > 0)
            {
                SendWhoIsChannels(info, user);
            }
            SendWhoIsServer(info, user);
            if (user.AwayMsg != null)
            {
                SendAwayMsg(info, user);
            }
            //if (user.Mode_O || user.Mode_o)
            //{
            //    SendWhoIsOperator(info, user);
            //}
            SendWhoIsIdle(info, user);
            SendEndOfWhoIs(info, user);
        }

        private void WhowasDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void KillDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void PingDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            SendPong(info);
        }

        private void PongDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }

        private void ErrorDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

        }
        #endregion

        #region Command Delegates RFC 2812 - optional
        private void AwayDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }

            if (args.Count == 0)
            {
                info.AwayMsg = null;
                SendUnAway(info);
            }
            else
            {
                info.AwayMsg = args[0];
                SendNowAway(info);
            }
        }

        private void rehashDelegate(UserInfo info, List<string> args)
        {

        }

        private void dieDelegate(UserInfo info, List<string> args)
        {

        }

        private void restartDelegate(UserInfo info, List<string> args)
        {

        }

        private void summonDelegate(UserInfo info, List<string> args)
        {

        }

        private void usersDelegate(UserInfo info, List<string> args)
        {

        }

        private void wallopsDelegate(UserInfo info, List<string> args)
        {

        }

        private void userhostDelegate(UserInfo info, List<string> args)
        {

        }

        private void IsonDelegate(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                SendNeedMoreParams(info);
                return;
            }

            var nickList = new List<string>();
            foreach (string nick in args)
            {
                if (nicks.ContainsKey(nick))
                    nickList.Add(nick);
            }
            SendIsOn(info, nickList);
        }
        #endregion

        #region Command Delegates - non standard Commands
        private void knockDelegate(UserInfo info, List<string> args)
        {

        }
        #endregion
    }
}
