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


using System.Text;

//namespace IrcD.ServerReplies
//{
//    class NumericServerReplies : ServerReplies
//    {
//        /// <summary>
//        /// Reply Code 001
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendWelcome(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Welcome);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Welcome to the Internet Relay Network ");
//            Response.Append(info.Usermask);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 002
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendYourHost(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.YourHost);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Your host is ");
//            Response.Append(Options.ServerName);
//            Response.Append(", running version ");
//            Response.Append(System.Reflection.Assembly.GetExecutingAssembly().FullName);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 003
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendCreated(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Created);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :This server was created ");
//            Response.Append(serverCreated);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 004
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendMyInfo(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.MyInfo);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :");
//            Response.Append(Options.ServerName);
//            Response.Append(" ");
//            Response.Append("ircD.Net.<version> <available user modes> <available channel modes>");
//            Response.Append(" ");
//            Response.Append(supportedUserModes.ToString());
//            Response.Append(" ");
//            Response.Append(supportedRanks.ToString() + supportedChannelModes);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 005
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendISupport(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ISupport);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            // TODO: features supported by server
//            Response.Append(" :are supported by this server");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 005 / 010
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendBounce(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Bounce);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            // TODO: bounce to which server
//            Response.Append(" :Try server <server name>, port <port number>");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 221
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendUserModeIs(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.UserModeIs);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(info.ModeString);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 301
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="awayUser"></param>
//        private void SendAwayMsg(UserInfo info, UserInfo awayUser)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Away);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(awayUser.Nick);
//            Response.Append(" :");
//            Response.Append(awayUser.AwayMsg);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 303
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="nickList"></param>
//        private void SendIsOn(UserInfo info, IEnumerable<string> nickList)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.IsOn);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :");
//            foreach (string nick in nickList)
//            {
//                Response.Append(nick + " ");
//            }
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 305
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendUnAway(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.UnAway);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You are no longer marked as being away");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 306
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendNowAway(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.NowAway);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You have been marked as being away");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }


//        /// <summary>
//        /// Reply 311
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendWhoIsUser(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsUser);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" ");
//            Response.Append(who.User);
//            Response.Append(" ");
//            Response.Append(who.Host);
//            Response.Append(" * :");
//            Response.Append(who.Realname);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 312
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendWhoIsServer(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsServer);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" ");
//            Response.Append(Options.ServerName); // TODO: when doing multiple IRC Server
//            Response.Append(" :");
//            Response.Append("IRC#Daemon Server Info"); // TODO: ServerInfo?
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 313
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendWhoIsOperator(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsOperator);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" :is an IRC operator");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 317
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendWhoIsIdle(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsIdle);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" ");
//            Response.Append((DateTime.Now - who.LastAction).TotalSeconds);
//            Response.Append(" :seconds idle");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 318
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendEndOfWhoIs(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.EndOfWhoIs);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" ");
//            Response.Append(" :End of WHOIS list");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 319
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="who"></param>
//        private void SendWhoIsChannels(UserInfo info, UserInfo who)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.WhoIsChannels);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(who.Nick);
//            Response.Append(" :");
//            foreach (var chan in who.Channels.Select(c => c.ChannelInfo))
//            {
//                Response.Append("");   // TODO: nickprefix (is in UPCI)
//                Response.Append(chan.Name);
//                Response.Append(" ");
//                // TODO: Split at max length
//            }
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 322
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        private void SendListItem(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.List);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" ");
//            Response.Append(chan.Users.Count);
//            Response.Append(" :");
//            Response.Append(chan.Topic);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 323
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendListEnd(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ListEnd);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :End of LIST");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 331
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        private void SendNoTopicReply(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.NoTopic);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :No topic is set");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 332
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        private void SendTopicReply(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Topic);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :" + chan.Topic);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 353
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        private void SendNamesReply(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.NamesReply);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" = ");
//            Response.Append(chan.Name);
//            Response.Append(" :");
//            foreach (UserPerChannelInfo upci in chan.Users.Values)
//            {
//                Response.Append(upci.Modes.NickPrefix);
//                Response.Append(upci.UserInfo.Nick);
//                Response.Append(" ");
//                // TODO: Split at max length
//            }
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 366
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        private void SendEndOfNamesReply(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.EndOfNames);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :End of NAMES list");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 372
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendMotd(UserInfo info)
//        {
//            foreach (string motdLine in Options.MOTD)
//            {
//                Response.Length = 0;
//                Response.Append(ServerPrefix);
//                Response.Append(" ");
//                Response.AppendFormat(NumericFormat, (int)ReplyCode.Motd);
//                Response.Append(" ");
//                Response.Append(info.Nick);
//                Response.Append(" :- ");
//                Response.Append(motdLine);
//                Response.Append(ServerCrLf);
//                info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//            }
//        }

//        /// <summary>
//        /// Reply Code 375
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendMotdStart(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.MotdStart);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :- ");
//            Response.Append(Options.ServerName);
//            Response.Append(" Message of the day -");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }


//        /// <summary>
//        /// Reply Code 376
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendMotdEnd(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.EndOfMotd);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :End of MOTD command");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 381
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendYouAreOper(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.YouAreOper);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You are now an IRC operator");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }
//        /// <summary>
//        /// Reply Code 383
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendYouAreService(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.YouAreService);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You are service ");
//            Response.Append(info.Nick);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 391
//        /// </summary>
//        /// <param name="info"></param>
//        private void SendTimeReply(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.Time);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(ServerPrefix);
//            Response.Append(" :");
//            Response.Append(DateTime.Now.ToString());
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }
//    }
//}
