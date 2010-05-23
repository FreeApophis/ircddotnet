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
using IrcD.Commands;
using IrcD.Modes;
using IrcD.Modes.ChannelModes;
using IrcD.Modes.ChannelRanks;
using IrcD.Modes.UserModes;
using IrcD.Utils;
using Mode = IrcD.Commands.Mode;
using Version = IrcD.Commands.Version;

namespace IrcD
{

    public class IrcDaemon
    {
        public const string ServerCrLf = "\r\n";
        const char PrefixCharacter = ':';
        const int MaxBufferSize = 2048;

        // Main Datastructures
        private readonly Dictionary<Socket, UserInfo> sockets = new Dictionary<Socket, UserInfo>();
        private readonly Dictionary<string, ChannelInfo> channels = new Dictionary<string, ChannelInfo>();
        private readonly Dictionary<string, Socket> nicks = new Dictionary<string, Socket>();
        public Dictionary<string, Socket> Nicks
        {
            get
            {
                return nicks;
            }
        }

        #region Modes
        private readonly RankList supportedRanks = new RankList();
        public dynamic SupportedRanks
        {
            get
            {
                return supportedRanks;
            }
        }
        private readonly ChannelModeList supportedChannelModes = new ChannelModeList();
        public dynamic SupportedChannelModes
        {
            get
            {
                return supportedChannelModes;
            }
        }
        private readonly UserModeList supportedUserModes = new UserModeList();
        public dynamic SupportedUserModes
        {
            get
            {
                return supportedUserModes;
            }
        }
        #endregion

        // Protocol Messages
        private readonly CommandList commands = new CommandList();
        public dynamic Commands
        {
            get
            {
                return commands;
            }
        }

        private readonly ServerReplies.ServerReplies replies;
        public ServerReplies.ServerReplies Replies
        {
            get
            {
                return replies;
            }
        }

        private bool connected = true;
        private byte[] buffer = new byte[MaxBufferSize];
        private EndPoint ep = new IPEndPoint(0, 0);
        private EndPoint localEp;

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
        public DateTime ServerCreated
        {
            get
            {
                return serverCreated;
            }
        }

        public IrcDaemon()
        {
            replies = new ServerReplies.ServerReplies(this);
            serverCreated = DateTime.Now;

            // Add Commands
            AddCommands();
            // Add Modes
            AddModes();
        }

        private void AddCommands()
        {
            commands.Add(new Admin(this));
            commands.Add(new Away(this));
            commands.Add(new Connect(this));
            commands.Add(new Die(this));
            commands.Add(new Error(this));
            commands.Add(new Info(this));
            commands.Add(new Invite(this));
            commands.Add(new IsOn(this));
            commands.Add(new Join(this));
            commands.Add(new Kick(this));
            commands.Add(new Kill(this));
            commands.Add(new Knock(this));
            commands.Add(new Links(this));
            commands.Add(new List(this));
            commands.Add(new ListUsers(this));
            commands.Add(new MessageOfTheDay(this));
            commands.Add(new Mode(this));
            commands.Add(new Names(this));
            commands.Add(new Nick(this));
            commands.Add(new Notice(this));
            commands.Add(new Oper(this));
            commands.Add(new Part(this));
            commands.Add(new Pass(this));
            commands.Add(new Ping(this));
            commands.Add(new Pong(this));
            commands.Add(new PrivateMessage(this));
            commands.Add(new Quit(this));
            commands.Add(new Rehash(this));
            commands.Add(new Restart(this));
            commands.Add(new ServerQuit(this));
            commands.Add(new Service(this));
            commands.Add(new ServiceList(this));
            commands.Add(new ServiceQuery(this));
            commands.Add(new Stats(this));
            commands.Add(new Summon(this));
            commands.Add(new Time(this));
            commands.Add(new Topic(this));
            commands.Add(new Trace(this));
            commands.Add(new User(this));
            commands.Add(new UserHost(this));
            commands.Add(new Version(this));
            commands.Add(new Wallops(this));
            commands.Add(new Who(this));
            commands.Add(new WhoIs(this));
            commands.Add(new WhoWas(this));

        }

        private void AddModes()
        {
            supportedRanks.Add(new ModeVoice());
            supportedRanks.Add(new ModeHalfOp());
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
                                Commands.Quit(sockets[s], new List<string> { "Socket reset by peer" });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Unknown ERROR: " + e.Message);
                        Logger.Log("Trace: " + e.StackTrace);
                    }
                }
                // pinger
                foreach (var user in from user in sockets.Where(s => s.Value.Registered)
                                     let interval = DateTime.Now.AddMinutes(-1)
                                     where user.Value.LastAction < interval && user.Value.LastPing < interval
                                     select user.Value)
                {
                    Commands.Ping(user);
                }

            }
        }

        public void Parser(string line, Socket sock, UserInfo info)
        {

#if DEBUG
            Logger.Log(line);
#endif

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

            commands.Handle(command, info, args);
        }

        #region Helper Methods
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

        //#region Server Messages
        //internal void SendNick(UserInfo sender, InfoBase receiver, string newnick)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" NICK ");
        //    commandSB.Append(newnick);
        //    receiver.WriteLine(commandSB);
        //}

        //internal void SendJoin(UserInfo sender, UserInfo receiver, ChannelInfo chan)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" JOIN ");
        //    commandSB.Append(chan.Name);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}

        //internal void SendPart(UserInfo sender, UserInfo receiver, ChannelInfo chan, string message)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" PART ");
        //    commandSB.Append(chan.Name);
        //    commandSB.Append(" :");
        //    commandSB.Append(message);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}

        //internal void SendTopic(UserInfo sender, UserInfo receiver, string chan, string newtopic)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" TOPIC ");
        //    commandSB.Append(chan);
        //    commandSB.Append(" :");
        //    commandSB.Append(newtopic);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}

        //internal void SendMode(UserInfo sender, UserInfo receiver, string target, string modestring)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" MODE ");
        //    commandSB.Append(target);
        //    commandSB.Append(" ");
        //    commandSB.Append(modestring);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}


        //internal void SendPrivMsg(UserInfo sender, UserInfo receiver, string target, string message)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" PRIVMSG ");
        //    commandSB.Append(target);
        //    commandSB.Append(" :");
        //    commandSB.Append(message);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}

        //internal void SendNotice(UserInfo sender, UserInfo receiver, string target, string message)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" NOTICE ");
        //    commandSB.Append(target);
        //    commandSB.Append(" :");
        //    commandSB.Append(message);
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}

        //internal void SendQuit(UserInfo sender, InfoBase receiver, string message)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(sender.Prefix);
        //    commandSB.Append(" QUIT :");
        //    commandSB.Append(message);

        //    receiver.WriteLine(commandSB);
        //}

        //internal void SendPong(UserInfo receiver)
        //{
        //    commandSB.Length = 0;
        //    commandSB.Append(ServerPrefix);
        //    commandSB.Append(" PONG ");
        //    commandSB.Append(ServerCrLf);
        //    receiver.Socket.Send(Encoding.UTF8.GetBytes(commandSB.ToString()));
        //}
        //#endregion

        //#region Command Delegates - RF2812 - required
        //internal void PassDelegate(UserInfo info, List<string> args)
        //{
        //    if (info.PassAccepted)
        //    {
        //        SendAlreadyRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }
        //    if (args[0] == Options.ServerPass)
        //    {
        //        info.PassAccepted = true;
        //        return;
        //    }
        //    SendPasswordMismatch(info);
        //}

        //internal void OperDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 2)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }

        //    // TODO: deny certain hosts OPER status
        //    if (false)
        //    {
        //        SendNoOperHost(info);
        //        return;
        //    }

        //    if (IsIrcOp(args[0], args[1]))
        //    {
        //        // TODO: new modes
        //        //info.Mode_o = true;
        //        //info.Mode_O = true;
        //        SendYouAreOper(info);
        //    }
        //    else
        //    {
        //        SendPasswordMismatch(info);
        //    }
        //}

        //internal void ModeDelegate(UserInfo info, List<string> args)
        //{
        //    // TODO: new modes
        //    //if (!info.Registered)
        //    //{
        //    //    SendNotRegistered(info);
        //    //    return;
        //    //}
        //    //if (args.Count == 0)
        //    //{
        //    //    SendNeedMoreParams(info);
        //    //    return;
        //    //}

        //    //if (ValidChannel(args[0]))
        //    //{
        //    //    if (!channels.ContainsKey(args[0]))
        //    //    {
        //    //        // TODO: Send Chan does not exist;
        //    //        return;
        //    //    }
        //    //    ChannelInfo chan = channels[args[0]];
        //    //    if (args.Count == 1)
        //    //    {
        //    //        // TODO: which modes should we send?
        //    //    }
        //    //    else
        //    //    {
        //    //        string reply = "";
        //    //        Dictionary<string, List<string>> compatibilityCache = null;
        //    //        foreach (ModeElement cmode in ParseChannelModes(args))
        //    //        {
        //    //            switch (cmode.Mode)
        //    //            {
        //    //                case 'a':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_a = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'b':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        if (cmode.Plus.Value)
        //    //                        {
        //    //                            chan.Mode_b.Add(cmode.Param);
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            chan.Mode_b.Remove(cmode.Param);
        //    //                        }
        //    //                    }
        //    //                    break;
        //    //                case 'e':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        if (cmode.Plus.Value)
        //    //                        {
        //    //                            chan.Mode_e.Add(cmode.Param);
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            chan.Mode_e.Remove(cmode.Param);
        //    //                        }
        //    //                    }
        //    //                    break;
        //    //                case 'h':
        //    //                    if (cmode.Param == null || cmode.Plus == null)
        //    //                        throw new ArgumentNullException();
        //    //                    if (chan.User.ContainsKey(cmode.Param))
        //    //                    {
        //    //                        chan.User[cmode.Param].Mode_h = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'i':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_i = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'I':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        if (cmode.Plus.Value)
        //    //                        {
        //    //                            chan.Mode_I.Add(cmode.Param);
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            chan.Mode_I.Remove(cmode.Param);
        //    //                        }
        //    //                    }
        //    //                    break;
        //    //                case 'k':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_k = cmode.Plus.Value ? cmode.Param : null;
        //    //                    }
        //    //                    break;
        //    //                case 'l':
        //    //                    if (cmode.Param == null)
        //    //                        throw new ArgumentNullException();
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        if (cmode.Plus.Value)
        //    //                        {
        //    //                            chan.Mode_l = int.Parse(cmode.Param);
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            chan.Mode_l = -1;
        //    //                        }
        //    //                    }
        //    //                    break;
        //    //                case 'm':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_m = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'n':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_n = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'o':
        //    //                    if (cmode.Param == null || cmode.Plus == null)
        //    //                        throw new ArgumentNullException();
        //    //                    if (chan.User.ContainsKey(cmode.Param))
        //    //                    {
        //    //                        chan.User[cmode.Param].Mode_o = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'O':
        //    //                    break;
        //    //                case 'p':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_p = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'q':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_q = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'r':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_r = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 's':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_s = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 't':
        //    //                    if (cmode.Plus != null)
        //    //                    {
        //    //                        chan.Mode_t = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                case 'v':
        //    //                    if (cmode.Param == null || cmode.Plus == null)
        //    //                        throw new ArgumentNullException();
        //    //                    if (chan.User.ContainsKey(cmode.Param))
        //    //                    {
        //    //                        chan.User[cmode.Param].Mode_v = cmode.Plus.Value;
        //    //                    }
        //    //                    break;
        //    //                default:
        //    //                    SendUnknownMode(info, chan, cmode.Mode);
        //    //                    continue;
        //    //            }

        //    //            if (Options.ClientCompatibilityMode)
        //    //            {
        //    //                compatibilityCache = compatibilityCache ?? new Dictionary<string, List<string>>();
        //    //                string key = ((cmode.Plus.HasValue) ? cmode.Plus.Value ? "+" : "-" : "") + cmode.Mode;
        //    //                if (compatibilityCache.ContainsKey(key))
        //    //                {
        //    //                    compatibilityCache[key].Add(cmode.Param ?? "");
        //    //                }
        //    //                else
        //    //                {
        //    //                    compatibilityCache.Add(key, new List<string> { cmode.Param ?? "" });
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                //TODO: this is very convinient, but mIRC and xchat cannot parse this (RTF RFC)
        //    //                if (cmode.Plus == null)
        //    //                {
        //    //                    reply += cmode.Mode + " ";
        //    //                }
        //    //                else if (cmode.Plus.Value)
        //    //                {
        //    //                    reply += "+" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
        //    //                }
        //    //                else
        //    //                {
        //    //                    reply += "-" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
        //    //                }
        //    //            }
        //    //        }
        //    //        if (Options.ClientCompatibilityMode && compatibilityCache != null)
        //    //        {
        //    //            foreach (var modes in compatibilityCache)
        //    //            {

        //    //                if (modes.Key[0] == '+' || modes.Key[0] == '-')
        //    //                {
        //    //                    reply += modes.Key[0];
        //    //                    reply += new string(modes.Key[1], modes.Value.Count);
        //    //                    foreach (var param in modes.Value)
        //    //                    {
        //    //                        if (param.Length > 0)
        //    //                        {
        //    //                            reply += " " + param;
        //    //                        }
        //    //                    }
        //    //                    reply += " ";
        //    //                }
        //    //                else
        //    //                {
        //    //                    reply += new string(modes.Key[0], modes.Value.Count) + " ";
        //    //                }
        //    //            }
        //    //        }

        //    //        if (reply.Length > 0)
        //    //        {
        //    //            foreach (UserPerChannelInfo upci in chan.User.Values)
        //    //            {
        //    //                SendMode(info, upci.Info, chan.Name, reply);
        //    //            }
        //    //        }

        //    //    }
        //    //}
        //    //else if (args[0] == info.Nick)
        //    //{
        //    //    if (args.Count == 1)
        //    //    {
        //    //        SendUserModeIs(info);
        //    //    }
        //    //    else
        //    //    {
        //    //        string reply = "";
        //    //        foreach (KeyValuePair<char, bool> umode in ParseUserMode(args[1]))
        //    //        {
        //    //            switch (umode.Key)
        //    //            {
        //    //                case 'i':
        //    //                    info.Mode_i = umode.Value;
        //    //                    break;
        //    //                case 'O':
        //    //                    if (!umode.Value)
        //    //                    {
        //    //                        info.Mode_O = false;
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        continue;
        //    //                    }
        //    //                    break;
        //    //                case 'o':
        //    //                    if (!umode.Value)
        //    //                    {
        //    //                        info.Mode_o = false;
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        continue;
        //    //                    }
        //    //                    break;
        //    //                case 'r':
        //    //                    if (umode.Value)
        //    //                    {
        //    //                        info.Mode_r = true;
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        continue;
        //    //                    }
        //    //                    break;
        //    //                case 's':
        //    //                    info.Mode_s = umode.Value;
        //    //                    break;
        //    //                case 'w':
        //    //                    info.Mode_w = umode.Value;
        //    //                    break;
        //    //                default:
        //    //                    SendUserModeUnknownFlag(info);
        //    //                    continue;
        //    //            }
        //    //            reply += ((umode.Value) ? "+" : "-") + umode.Key;

        //    //        }
        //    //        if (reply.Length > 0)
        //    //        {
        //    //            SendMode(info, info, info.Nick, reply);
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    SendUsersDoNotMatch(info);
        //    //}
        //}

        //internal void ServiceDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (info.Registered)
        //    {
        //        SendAlreadyRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 6)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }
        //    if (!ValidNick(args[0]))
        //    {
        //        SendErroneousNickname(info, args[0]);
        //        return;
        //    }
        //    if (nicks.ContainsKey(args[0]))
        //    {
        //        SendNicknameInUse(info, args[0]);
        //        return;
        //    }

        //    info.Nick = args[0];
        //    nicks.Add(info.Nick, info.Socket);

        //    info.User = "service";
        //    info.IsService = true;
        //    info.Registered = true;

        //    SendYouAreService(info);
        //    SendYourHost(info);
        //    SendMyInfo(info);
        //}

        //internal void QuitDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //    string message = (args.Count > 0) ? args[0] : "";

        //    foreach (ChannelInfo channelInfo in info.Channels.Select(upci => upci.ChannelInfo))
        //    {
        //        SendQuit(info, channelInfo, message);
        //    }

        //    foreach (ChannelInfo chaninfo in info.Channels.Select(upci => upci.ChannelInfo))
        //    {
        //        chaninfo.Users.Remove(info.Nick);
        //    }

        //    sockets.Remove(info.Socket);
        //    nicks.Remove(info.Nick);
        //    info.Socket.Close();

        //}

        //internal void SquitDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //internal void JoinDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }
        //    if (args[0] == "0")
        //    {
        //        var partargs = new List<string>();
        //        // this is a part all channels, this is plainly stupid to handle PARTS in a join message.
        //        // we won't handle that, we give it to the part handler! YO! why not defining a /part * instead of /join 0
        //        commandSB.Length = 0; bool first = true;
        //        foreach (ChannelInfo ci in info.Channels.Select(upci => upci.ChannelInfo))
        //        {
        //            if (first)
        //            {
        //                first = false;
        //            }
        //            else
        //            {
        //                commandSB.Append(",");
        //            }
        //            commandSB.Append(ci.Name);
        //        }
        //        partargs.Add(commandSB.ToString());
        //        partargs.Add("Left all channels");
        //        PartDelegate(info, partargs);
        //        return;
        //    }

        //    IEnumerable<string> keys;
        //    if (args.Count > 1)
        //    {
        //        keys = GetSubArgument(args[1]);
        //    }
        //    else
        //    {
        //        keys = new List<string>();
        //    }

        //    foreach (string ch in GetSubArgument(args[0]))
        //    {
        //        ChannelInfo chan;
        //        if (channels.ContainsKey(ch))
        //        {
        //            chan = channels[ch];
        //            // TODO: new modes
        //            // Check for (+l)
        //            //if ((chan.Mode_l != -1) && (chan.Mode_l <= chan.User.Count))
        //            //{
        //            //    SendChannelIsFull(info, chan);
        //            //    return;
        //            //}

        //            // Check for (+k)
        //            //if (!string.IsNullOrEmpty(chan.Mode_k))
        //            //{
        //            //    bool j = false;
        //            //    foreach (string key in keys)
        //            //    {
        //            //        if (key == chan.Mode_k)
        //            //        {
        //            //            j = true;
        //            //        }
        //            //    }
        //            //    if (!j)
        //            //    {
        //            //        SendBadChannelKey(info, chan);
        //            //        return;
        //            //    }
        //            //}

        //            // Check for (+i)
        //            //if (chan.Mode_i)
        //            //{
        //            //    // TODO: implement invite
        //            //    SendInviteOnlyChannel(info, chan);
        //            //}

        //            // Check for (+b) (TODO)
        //            if (false)
        //            {
        //                SendBannedFromChannel(info, chan);
        //            }
        //        }
        //        else
        //        {
        //            chan = new ChannelInfo(ch, this);
        //            channels.Add(chan.Name, chan);
        //        }

        //        var chanuser = new UserPerChannelInfo(info, chan);
        //        chan.Users.Add(info.Nick, chanuser);
        //        info.Channels.Add(chanuser);

        //        foreach (UserPerChannelInfo upci in chan.Users.Values)
        //        {
        //            SendJoin(info, upci.UserInfo, chan);
        //        }


        //        if (string.IsNullOrEmpty(chan.Topic))
        //        {
        //            SendNoTopicReply(info, chan);
        //        }
        //        else
        //        {
        //            SendTopicReply(info, chan);
        //        }
        //        SendNamesReply(chanuser.UserInfo, chan);
        //        SendEndOfNamesReply(info, chan);
        //    }

        //}

        //internal void PartDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }
        //    string message = (args.Count > 1) ? args[1] : "";

        //    foreach (string ch in GetSubArgument(args[0]))
        //    {
        //        if (channels.ContainsKey(ch))
        //        {
        //            ChannelInfo chan = channels[ch];
        //            //if (info.Channels.Contains(chan))
        //            //{
        //            //    foreach (UserPerChannelInfo upci in chan.User.Values)
        //            //    {
        //            //        SendPart(info, upci.Info, chan, message);
        //            //    }
        //            //    chan.User.Remove(info.Nick);
        //            //    info.Channels.Remove(chan);
        //            //}
        //            //else
        //            //{
        //            //    SendNotOnChannel(info, ch);
        //            //    continue;
        //            //}
        //        }
        //        else
        //        {
        //            SendNoSuchChannel(info, ch);
        //            continue;
        //        }
        //    }
        //}

        //internal void TopicDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    switch (args.Count)
        //    {
        //        case 0:
        //            SendNeedMoreParams(info);
        //            break;
        //        case 1:
        //            if (channels.ContainsKey(args[0]))
        //            {
        //                if (string.IsNullOrEmpty(channels[args[0]].Topic))
        //                {
        //                    SendNoTopicReply(info, channels[args[0]]);
        //                }
        //                else
        //                {
        //                    SendTopicReply(info, channels[args[0]]);
        //                }
        //            }
        //            else
        //            {
        //                SendNoSuchChannel(info, args[0]);
        //            }
        //            break;
        //        case 2:
        //            if (channels.ContainsKey(args[0]))
        //            {
        //                channels[args[0]].Topic = args[1];
        //                foreach (UserPerChannelInfo upci in channels[args[0]].Users.Values)
        //                {
        //                    SendTopic(info, upci.UserInfo, args[0], args[1]);
        //                }
        //            }
        //            break;
        //        default:
        //            // TODO: Protocol error too many params
        //            break;

        //    }
        //}

        //internal void NamesDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //    if (args.Count < 1)
        //    {
        //        // TODO: list all visible users
        //        return;
        //    }

        //    //TODO: taget parameter
        //    foreach (string ch in GetSubArgument(args[0]))
        //    {
        //        if (channels.ContainsKey(ch))
        //        {
        //            SendNamesReply(info, channels[ch]);
        //        }
        //    }

        //}

        //private void ListDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    // TODO: special LIST commands / implemented is full list
        //    foreach (ChannelInfo ci in channels.Values)
        //    {
        //        SendListItem(info, ci);
        //    }
        //    SendListEnd(info);
        //}

        //private void InviteDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void KickDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //}

        //private void PrivmsgDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNoRecipient(info, "PRIVMSG");
        //        return;
        //    }
        //    if (args.Count < 2)
        //    {
        //        SendNoTextToSend(info);
        //        return;
        //    }

        //    // TODO: idle timer, which commands reset them?
        //    info.LastAction = DateTime.Now;

        //    if (ValidChannel(args[0]))
        //    {
        //        if (channels.ContainsKey(args[0]))
        //        {
        //            ChannelInfo chan = channels[args[0]];
        //            //if (chan.Mode_n && (!info.Channels.Contains(chan)) /*TODO: banned user cannot send even without Mode_n set*/)
        //            //{
        //            //    SendCannotSendToChannel(info, chan.Name);
        //            //    return;
        //            //}
        //            //if (!chan.Mode_m || (chan.Mode_m && info.Channels.Contains(chan) &&
        //            //                    (chan.User[info.Nick].Mode_v || chan.User[info.Nick].Mode_h || chan.User[info.Nick].Mode_o)))
        //            //{
        //            //    foreach (UserPerChannelInfo upci in chan.User.Values)
        //            //    {
        //            //        if (upci.Info.Nick != info.Nick)
        //            //        {
        //            //            SendPrivMsg(info, upci.Info, chan.Name, args[1]);
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    SendCannotSendToChannel(info, chan.Name);
        //            //}
        //        }
        //        else
        //        {
        //            SendNoSuchChannel(info, args[0]);
        //        }
        //    }
        //    else if (ValidNick(args[0]))
        //    {
        //        if (nicks.ContainsKey(args[0]))
        //        {
        //            UserInfo user = sockets[nicks[args[0]]];
        //            if (user.AwayMsg != null)
        //            {
        //                SendAwayMsg(info, user);
        //            }
        //            SendPrivMsg(info, user, user.Nick, args[1]);
        //        }
        //        else
        //        {
        //            SendNoSuchNick(info, args[0]);
        //        }
        //    }
        //    else
        //    {
        //        SendNoSuchNick(info, args[0]);
        //    }
        //}

        //private void NoticeDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNoRecipient(info, "NOTICE");
        //        return;
        //    }
        //    if (args.Count < 2)
        //    {
        //        SendNoTextToSend(info);
        //        return;
        //    }

        //    // TODO: idle timer, which commands reset them?
        //    info.LastAction = DateTime.Now;

        //    if (ValidChannel(args[0]))
        //    {
        //        if (channels.ContainsKey(args[0]))
        //        {
        //            //ChannelInfo chan = channels[args[0]];
        //            //if (chan.Mode_n && (!info.Channels.Contains(chan)) /*TODO: banned user cannot send even without Mode_n set*/)
        //            //{
        //            //    SendCannotSendToChannel(info, chan.Name);
        //            //    return;
        //            //}
        //            //if (!chan.Mode_m || (chan.Mode_m && info.Channels.Contains(chan) &&
        //            //                    (chan.User[info.Nick].Mode_v || chan.User[info.Nick].Mode_h || chan.User[info.Nick].Mode_o)))
        //            //{
        //            //    foreach (UserPerChannelInfo upci in chan.User.Values)
        //            //    {
        //            //        if (upci.Info.Nick != info.Nick)
        //            //        {
        //            //            SendNotice(info, upci.Info, chan.Name, args[1]);
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    SendCannotSendToChannel(info, chan.Name);
        //            //}
        //        }
        //        else
        //        {
        //            SendNoSuchChannel(info, args[0]);
        //        }
        //    }
        //    else if (ValidNick(args[0]))
        //    {
        //        if (nicks.ContainsKey(args[0]))
        //        {
        //            UserInfo user = sockets[nicks[args[0]]];
        //            if (user.AwayMsg != null)
        //            {
        //                SendAwayMsg(info, user);
        //            }
        //            SendNotice(info, user, user.Nick, args[1]);
        //        }
        //        else
        //        {
        //            SendNoSuchNick(info, args[0]);
        //        }
        //    }
        //    else
        //    {
        //        SendNoSuchNick(info, args[0]);
        //    }
        //}

        //private void MOTDDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //    // TODO: parameter 1 parsing

        //    if (Options.MOTD.Count == 0)
        //    {
        //        SendNoMotd(info);
        //    }
        //    else
        //    {
        //        SendMotdStart(info);
        //        SendMotd(info);
        //        SendMotdEnd(info);
        //    }
        //}

        //private void LusersDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void VersionDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void StatsDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void LinksDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void TimeDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //    //TODO: Parse Server Argument
        //    SendTimeReply(info);
        //}

        //private void ConnectDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void TraceDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void AdminDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void InfoDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void ServlistDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void SqueryDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void WhoDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void WhoisDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }
        //    if (!nicks.ContainsKey(args[0]))
        //    {
        //        SendNoSuchNick(info, args[0]);
        //        return;
        //    }

        //    UserInfo user = sockets[nicks[args[0]]];
        //    SendWhoIsUser(info, user);
        //    if (user.Channels.Count > 0)
        //    {
        //        SendWhoIsChannels(info, user);
        //    }
        //    SendWhoIsServer(info, user);
        //    if (user.AwayMsg != null)
        //    {
        //        SendAwayMsg(info, user);
        //    }
        //    //if (user.Mode_O || user.Mode_o)
        //    //{
        //    //    SendWhoIsOperator(info, user);
        //    //}
        //    SendWhoIsIdle(info, user);
        //    SendEndOfWhoIs(info, user);
        //}

        //private void WhowasDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void KillDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void PongDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}

        //private void ErrorDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //}
        //#endregion

        //#region Command Delegates RFC 2812 - optional
        //private void AwayDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }

        //    if (args.Count == 0)
        //    {
        //        info.AwayMsg = null;
        //        SendUnAway(info);
        //    }
        //    else
        //    {
        //        info.AwayMsg = args[0];
        //        SendNowAway(info);
        //    }
        //}

        //private void rehashDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void dieDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void restartDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void summonDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void usersDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void wallopsDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void userhostDelegate(UserInfo info, List<string> args)
        //{

        //}

        //private void IsonDelegate(UserInfo info, List<string> args)
        //{
        //    if (!info.Registered)
        //    {
        //        SendNotRegistered(info);
        //        return;
        //    }
        //    if (args.Count < 1)
        //    {
        //        SendNeedMoreParams(info);
        //        return;
        //    }

        //    var nickList = new List<string>();
        //    foreach (string nick in args)
        //    {
        //        if (nicks.ContainsKey(nick))
        //            nickList.Add(nick);
        //    }
        //    SendIsOn(info, nickList);
        //}
        //#endregion

        //#region Command Delegates - non standard Commands
        //private void knockDelegate(UserInfo info, List<string> args)
        //{

        //}
        //#endregion
    }
}
