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
using System.Text;
using IrcD.Channel;
using IrcD.Modes.UserModes;
using IrcD.Utils;

namespace IrcD.ServerReplies
{
    public class ServerReplies
    {
        private const string NumericFormat = "{0:000}";
        private readonly StringBuilder response = new StringBuilder();
        private readonly IrcDaemon ircDaemon;

        public ServerReplies(IrcDaemon ircDaemon)
        {
            this.ircDaemon = ircDaemon;
        }

        #region Helper Methods
        public void RegisterComplete(UserInfo info)
        {
            SendWelcome(info);
            SendYourHost(info);
            SendCreated(info);
            SendMyInfo(info);
            SendISupport(info);
            if (string.IsNullOrEmpty(ircDaemon.Options.MessageOfTheDay))
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

        private void BuildMessageHeader(UserInfo info, ReplyCode code)
        {
            response.Length = 0;
            response.Append(ircDaemon.ServerPrefix);
            response.Append(" ");
            response.AppendFormat(NumericFormat, (int)code);
            response.Append(" ");
            response.Append(info.Nick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="info"></param>
        /// <param name="features"></param>
        /// <param name="postfix"></param>
        private static void SendSplitted(string prefix, UserInfo info, IEnumerable<string> features, string postfix)
        {
            var daemon = info.IrcDaemon;
            var currentLine = new StringBuilder();
            var postfixlength = postfix != null ? 2 + postfix.Length : 0;

            currentLine.Append(prefix);

            var first = true;
            foreach (var feature in features)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    currentLine.Append(" ");
                }
                if (feature.StartsWith("LANGUAGE"))
                {
                    currentLine.Append(feature);

                    foreach (var language in GoogleTranslate.Languages.Keys)
                    {
                        if (currentLine.Length + 1 + language.Length + postfixlength > daemon.Options.MaxLineLength)
                        {
                            if (postfix != null)
                            {
                                currentLine.Append(" :");
                                currentLine.Append(postfix);
                            }
                            info.WriteLine(currentLine);
                            currentLine.Length = 0;
                            currentLine.Append(prefix);
                            currentLine.Append(feature);
                        }
                        currentLine.Append(",");
                        currentLine.Append(language);
                    }
                    continue;
                }
                if (currentLine.Length + 1 + feature.Length + postfixlength > daemon.Options.MaxLineLength)
                {
                    if (postfix != null)
                    {
                        currentLine.Append(" :");
                        currentLine.Append(postfix);
                    }
                    info.WriteLine(currentLine);
                    currentLine.Length = 0;
                    currentLine.Append(prefix);
                }
                currentLine.Append(feature);
            }

            if (postfix != null)
            {
                currentLine.Append(" :");
                currentLine.Append(postfix);
            }
            info.WriteLine(currentLine);

        }
        #endregion

        /// <summary>
        /// Reply Code 001
        /// </summary>
        /// <param name="info"></param>
        public void SendWelcome(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Welcome);

            response.Append(" :Welcome to the ");
            response.Append(info.IrcDaemon.Options.NetworkName);
            response.Append(" IRC Network ");
            response.Append(info.Usermask);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 002
        /// </summary>
        /// <param name="info"></param>
        public void SendYourHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YourHost);

            response.Append(" :Your host is ");
            response.Append(ircDaemon.Options.ServerName);
            response.Append(", running version ");
            response.Append(System.Reflection.Assembly.GetExecutingAssembly().FullName);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 003
        /// </summary>
        /// <param name="info"></param>
        public void SendCreated(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Created);

            response.Append(" :This server was created ");
            response.Append(ircDaemon.ServerCreated);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 004
        /// </summary>
        /// <param name="info"></param>
        public void SendMyInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.MyInfo);

            response.Append(" :");
            response.Append(ircDaemon.Options.ServerName);
            response.Append(" ");
            response.Append("ircD.Net.");
            response.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            response.Append(" ");
            response.Append(ircDaemon.SupportedUserModes.ToString());
            response.Append(" ");
            response.Append(ircDaemon.SupportedRanks.ToString() + ircDaemon.SupportedChannelModes);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 005
        /// </summary>
        /// <param name="info"></param>
        public void SendISupport(UserInfo info)
        {
            // ISUPPORT never went to an RFC, but this is based on draft03.
            const string version = "draft03";

            var features = new List<string>();
            var daemon = info.IrcDaemon;

            features.Add("STD=" + version);
            features.Add("PREFIX=" + daemon.SupportedRanks.ToPrefixList());
            features.Add("CHANMODES=" + daemon.SupportedChannelModes.ToParameterList());
            features.Add("CHANTYPES=" + daemon.SupportedChannelTypes.Select(type => type.Value.Prefix).Concatenate(string.Empty));
            features.Add("CHANLIMIT=" + daemon.SupportedChannelTypes.Select(c => c.Value.Prefix + ":" + c.Value.MaxJoinedAllowed).Concatenate(","));
            features.AddRange(daemon.SupportedChannelModes.SelectMany(m => m.Value.Support(info.IrcDaemon)));
            features.Add("NETWORK=" + daemon.Options.NetworkName);
            features.Add("CASEMAPPING=" + daemon.Options.IrcCaseMapping.ToDescription());
            // TODO: Group by same MaxJoinedAllowed
            features.Add("CHANNELLEN=" + daemon.Options.MaxChannelLength);
            features.Add("NICKLEN=" + daemon.Options.MaxNickLength);
            features.Add("MAXNICKLEN=" + daemon.Options.MaxNickLength);
            features.Add("TOPICLEN=" + daemon.Options.MaxTopicLength);
            features.Add("KICKLEN=" + daemon.Options.MaxKickLength);
            features.Add("AWAYLEN=" + daemon.Options.MaxAwayLength);
            features.AddRange(daemon.Commands.Supported());

            if (daemon.Options.IrcMode != IrcMode.Rfc1459)
            {
                features.Add("RFC2812");
            }

            BuildMessageHeader(info, ReplyCode.ISupport);
            response.Append(" ");

            SendSplitted(response.ToString(), info, features, "are supported by this server");
        }

        /// <summary>
        /// Reply Code 005 / 010
        /// </summary>
        /// <param name="info"></param>
        public void SendBounce(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Bounce);

            // TODO: bounce to which server
            response.Append(" :Try server <server name>, port <port number>");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 221
        /// </summary>
        /// <param name="info"></param>
        public void SendUserModeIs(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.UserModeIs);

            response.Append(" ");
            response.Append(info.ModeString);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 251
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserClient(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserClient);

            // TODO: implement this
            response.Append(":There are <integer> users and <integer> services on <integer> servers");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 252
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserOp(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserOp);

            // TODO: implement this
            response.Append("<integer> :operator(s) online");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 253
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserUnknown(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserUnknown);

            // TODO: implement this
            response.Append("<integer> :unknown connection(s)");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 254
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserChannels(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserChannels);

            // TODO: implement this
            response.Append("<integer> :channels formed");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 255
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserMe(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserMe);

            // TODO: implement this
            response.Append(":I have <integer> clients and <integer> servers");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 301
        /// </summary>
        /// <param name="info"></param>
        /// <param name="awayUser"></param>
        public void SendAwayMessage(UserInfo info, UserInfo awayUser)
        {
            BuildMessageHeader(info, ReplyCode.Away);

            response.Append(" ");
            response.Append(awayUser.Nick);
            response.Append(" :");
            response.Append(awayUser.AwayMessage);

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply 302
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userInfos"></param>
        public void SendUserHost(UserInfo info, List<UserInfo> userInfos)
        {
            BuildMessageHeader(info, ReplyCode.UserHost);

            response.Append(" :");

            var userHosts = new List<string>();

            foreach (var userInfo in userInfos)
            {
                var userHost = new StringBuilder();

                userHost.Append(userInfo.Nick);

                if (userInfo.Modes.Exist<ModeOperator>() || userInfo.Modes.Exist<ModeLocalOperator>())
                {
                    userHost.Append("*");
                }

                userHost.Append("=");
                userHost.Append(userInfo.Modes.Exist<ModeAway>() ? "-" : "+");
                userHost.Append(userInfo.User);
                userHost.Append("@");
                userHost.Append(userInfo.Host);

                userHosts.Add(userHost.ToString());

            }

            SendSplitted(response.ToString(), info, userHosts, null);
        }

        /// <summary>
        /// Reply 303
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nickList"></param>
        public void SendIsOn(UserInfo info, IEnumerable<string> nickList)
        {
            BuildMessageHeader(info, ReplyCode.IsOn);

            response.Append(" :");

            SendSplitted(response.ToString(), info, nickList, null);
        }

        /// <summary>
        /// Reply 305
        /// </summary>
        /// <param name="info"></param>
        public void SendUnAway(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.UnAway);

            response.Append(" :You are no longer marked as being away");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 306
        /// </summary>
        /// <param name="info"></param>
        public void SendNowAway(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.NowAway);

            response.Append(" :You have been marked as being away");

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply 311
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsUser(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsUser);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" ");
            response.Append(who.User);
            response.Append(" ");
            response.Append(who.Host);
            response.Append(" * :");
            response.Append(who.RealName);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 312
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsServer(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsServer);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" ");
            response.Append(ircDaemon.Options.ServerName); // TODO: when doing multiple IRC Server
            response.Append(" :");
            response.Append(ircDaemon.Options.NetworkName); // TODO: ServerInfo?

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 313
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsOperator(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsOperator);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" :is an IRC operator");

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply 315
        /// </summary>
        /// <param name="info"></param>
        /// <param name="mask"></param>
        public void SendEndOfWho(UserInfo info, string mask)
        {
            BuildMessageHeader(info, ReplyCode.EndOfWho);

            response.Append(" ");
            response.Append(mask);
            response.Append(" :End of /WHO list.");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 317
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsIdle(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsIdle);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" ");
            if (ircDaemon.Options.IrcMode == IrcMode.Modern)
            {
                response.Append((int)((DateTime.Now - who.LastAction).TotalSeconds));
                response.Append(" ");
                response.Append(info.Created.ToUnixTime());
                response.Append(" :seconds idle, signon time");

            }
            else
            {
                response.Append((int)((DateTime.Now - who.LastAction).TotalSeconds));
                response.Append(" :seconds idle");
            }

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 318
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendEndOfWhoIs(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.EndOfWhoIs);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" ");
            response.Append(" :End of WHOIS list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 319
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsChannels(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsChannels);

            response.Append(" ");
            response.Append(who.Nick);
            response.Append(" :");

            SendSplitted(response.ToString(), info, who.UserPerChannelInfos.Select(ucpi => ucpi.Modes.NickPrefix + ucpi.ChannelInfo.Name), null);
        }

        /// <summary>
        /// Reply Code 321
        /// </summary>
        /// <param name="info"></param>
        public void SendListStart(UserInfo info)
        {
            if (info.IrcDaemon.Options.IrcMode != IrcMode.Rfc1459)
                throw new NotSupportedException("This message is only valid in RFC 1459 mode");

            BuildMessageHeader(info, ReplyCode.ListStart);

            response.Append(" Channel :Users Name");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 322
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendListItem(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.List);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(chan.UserPerChannelInfos.Count);
            response.Append(" :");
            response.Append(chan.Topic);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 323
        /// </summary>
        /// <param name="info"></param>
        public void SendListEnd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListEnd);

            response.Append(" :End of LIST");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 324
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelModeIs(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ChannelModeIs);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(chan.ModeString);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 331
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNoTopicReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.NoTopic);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :No topic is set");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 332
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendTopicReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.Topic);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :");
            response.Append(chan.Topic);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 341
        /// </summary>
        /// <param name="info"></param>
        /// <param name="invited"></param>
        /// <param name="channel"></param>
        public void SendInviting(UserInfo info, UserInfo invited, string channel)
        {
            BuildMessageHeader(info, ReplyCode.Inviting);

            // The RFC Tells the order should be <channel> <nick> however xchat and the servers I tested say it is: <nick> <channel> 
            // This is one more ridiculous RFC mistake without any errata.

            if (ircDaemon.Options.IrcMode == IrcMode.Rfc1459 || ircDaemon.Options.IrcMode == IrcMode.Rfc2810)
            {
                response.Append(" ");
                response.Append(channel);
            }

            response.Append(" ");
            response.Append(invited.Nick);

            if (ircDaemon.Options.IrcMode == IrcMode.Modern)
            {
                response.Append(" ");
                response.Append(channel);
            }

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 342
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        public void SendSummoning(UserInfo info, string user)
        {
            BuildMessageHeader(info, ReplyCode.Summoning);

            response.Append(" ");
            response.Append(user);
            response.Append(" :User summoned to irc");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 346
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mask"></param>
        public void SendInviteList(UserInfo info, ChannelInfo chan, string mask)
        {
            BuildMessageHeader(info, ReplyCode.InviteList);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(mask);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 347
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfInviteList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfInviteList);


            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :End of channel invite list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 348
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mask"></param>
        public void SendExceptionList(UserInfo info, ChannelInfo chan, string mask)
        {
            BuildMessageHeader(info, ReplyCode.ExceptionList);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(mask);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 349
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfExceptionList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfExceptionList);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :End of channel exception list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 351
        /// </summary>
        /// <param name="info"></param>
        public void SendVersion(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Version);

            response.Append(" ");
            response.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            response.Append(" ");
            response.Append(ircDaemon.Options.ServerName);
            response.Append(" :" + System.Reflection.Assembly.GetExecutingAssembly().FullName);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 352
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoReply(UserInfo info, UserPerChannelInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoReply);

            response.Append(" ");
            response.Append(who.ChannelInfo.Name);
            response.Append(" ");
            response.Append(who.UserInfo.User);
            response.Append(" ");
            response.Append(who.UserInfo.Host);
            response.Append(" ");

            // TODO Server to Server
            response.Append(ircDaemon.Options.ServerName);
            response.Append(" ");
            response.Append(who.UserInfo.Nick);
            response.Append(" ");
            response.Append(who.UserInfo.Modes.Exist<ModeAway>() ? "G" : "H");
            response.Append(who.UserInfo.Modes.Exist<ModeOperator>() || who.UserInfo.Modes.Exist<ModeLocalOperator>() ? "*" : string.Empty);
            response.Append(who.Modes.NickPrefix);
            response.Append(" :");

            // TODO: Server to Server
            //response.Append(who.Server.Hops);            
            response.Append(0);
            response.Append(" ");
            response.Append(who.UserInfo.RealName);

            //TODO: append d if deaf - add Deaf (such as Mode X / W on undernet)

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 353
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNamesReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.NamesReply);
            response.Append(" ");
            response.Append(chan.NamesPrefix);
            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :");

            SendSplitted(response.ToString(), info, chan.UserPerChannelInfos.Values.Select(upci => upci.Modes.NickPrefix + upci.UserInfo.Nick), null);
        }

        /// <summary>
        /// Reply Code 366
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfNamesReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfNames);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :End of NAMES list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 367
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mask"></param>
        public void SendBanList(UserInfo info, ChannelInfo chan, string mask)
        {
            BuildMessageHeader(info, ReplyCode.BanList);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(mask);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 368
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfBanList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfBanList);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :End of channel ban list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 371
        /// </summary>
        /// <param name="info"></param>
        public void SendInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Info);

            response.Append(" :");
            response.Append("TODO");

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply Code 372
        /// </summary>
        /// <param name="info"></param>
        public void SendMotd(UserInfo info)
        {
            foreach (var motdLine in ircDaemon.Options.MessageOfTheDay.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                BuildMessageHeader(info, ReplyCode.Motd);

                response.Append(" :- ");
                response.Append(motdLine);

                info.WriteLine(response);
            }
        }

        /// <summary>
        /// Reply Code 374
        /// </summary>
        /// <param name="info"></param>
        public void SendEndOfInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.EndOfInfo);

            response.Append(" :End of /INFO list");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 375
        /// </summary>
        /// <param name="info"></param>
        public void SendMotdStart(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.MotdStart);

            response.Append(" :- ");
            response.Append(ircDaemon.Options.ServerName);
            response.Append(" Message of the day -");

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply Code 376
        /// </summary>
        /// <param name="info"></param>
        public void SendMotdEnd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.EndOfMotd);

            response.Append(" :End of MOTD command");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 381
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreOper(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YouAreOper);

            response.Append(" :You are now an IRC operator");

            info.WriteLine(response);
        }
        /// <summary>
        /// Reply Code 383
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreService(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YouAreService);

            response.Append(" :You are service ");
            response.Append(info.Nick);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 391
        /// </summary>
        /// <param name="info"></param>
        public void SendTimeReply(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Time);

            response.Append(" ");
            response.Append(ircDaemon.ServerPrefix);
            response.Append(" :");
            response.Append(DateTime.Now.ToString());

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply Code 401
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendNoSuchNick(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchNickname);

            response.Append(" ");
            response.Append(nick);
            response.Append(" :No such nick/channel");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 402
        /// </summary>
        /// <param name="info"></param>
        /// <param name="server"></param>
        public void SendNoSuchServer(UserInfo info, string server)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchServer);

            response.Append(" ");
            response.Append(server);
            response.Append(" :No such server");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 403
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void SendNoSuchChannel(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchChannel);

            response.Append(" ");
            response.Append(channel);
            response.Append(" :No such channel");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 404
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        public void SendCannotSendToChannel(UserInfo info, string channel, string message = "Cannot send to channel")
        {
            BuildMessageHeader(info, ReplyCode.ErrorCannotSendToChannel);

            response.Append(" ");
            response.Append(channel);
            response.Append(" :");
            response.Append(message);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 405
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void Send(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorTooManyChannels);

            response.Append(" ");
            response.Append(channel);
            response.Append(" :You have joined too many channels");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 411
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public void SendNoRecipient(UserInfo info, string command)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoRecipient);

            response.Append(" :No recipient given (");
            response.Append(command);
            response.Append(")");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 412
        /// </summary>
        /// <param name="info"></param>
        public void SendNoTextToSend(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoTextToSend);

            response.Append(" :No text to send");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 421
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public void SendUnknownCommand(UserInfo info, string command)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUnknownCommand);

            response.Append(" ");
            response.Append(command);
            response.Append(" :Unknown command");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 422
        /// </summary>
        /// <param name="info"></param>
        public void SendNoMotd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoMotd);

            response.Append(" :MOTD File is missing");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 431
        /// </summary>
        /// <param name="info"></param>
        public void SendNoNicknameGiven(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoNicknameGiven);

            response.Append(" :No nickname given");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 432
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendErroneousNickname(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorErroneusNickname);

            response.Append(" :Erroneous nickname");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 433
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendNicknameInUse(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNicknameInUse);

            response.Append(" :Nickname is already in use");

            info.WriteLine(response);
        }


        /// <summary>
        /// Reply Code 441
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        /// <param name="nick"></param>
        public void SendUserNotInChannel(UserInfo info, string channel, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUserNotInChannel);

            response.Append(" ");
            response.Append(nick);
            response.Append(" ");
            response.Append(channel);
            response.Append(" :They aren't on that channel");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 442
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void SendNotOnChannel(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNotOnChannel);

            response.Append(" ");
            response.Append(channel);
            response.Append(" :You're not on that channel");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 443
        /// </summary>
        /// <param name="info"></param>
        /// <param name="invited"></param>
        /// <param name="channel"></param>
        public void SendUserOnChannel(UserInfo info, UserInfo invited, ChannelInfo channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUserOnChannel);

            response.Append(" ");
            response.Append(invited.Nick);
            response.Append(" ");
            response.Append(channel.Name);
            response.Append(" :is already on channel");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 445
        /// </summary>
        /// <param name="info"></param>
        public void SendSummonDisabled(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorSummonDisabled);

            response.Append(" :SUMMON has been disabled");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 446
        /// </summary>
        /// <param name="info"></param>
        public void SendUsersDisabled(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUsersDisabled);

            response.Append(" :USERS has been disabled");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 451
        /// </summary>
        /// <param name="info"></param>
        public void SendNotRegistered(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNotRegistered);

            response.Append(" :You have not registered");

            info.WriteLine(response);
        }

        /// <summary>
        /// Numeric Reply 461
        /// </summary>
        /// <param name="info"></param>
        public void SendNeedMoreParams(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNeedMoreParams);

            response.Append(" :Not enough parameters");

            info.WriteLine(response);
        }

        /// <summary>
        /// Numeric Reply 462
        /// </summary>
        /// <param name="info"></param>
        public void SendAlreadyRegistered(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorAlreadyRegistered);

            response.Append(" :Unauthorized command (already registered)");

            info.WriteLine(response);
        }

        /// <summary>
        /// Numeric Reply 463
        /// </summary>
        /// <param name="info"></param>
        public void SendNoPermissionForHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoPermissionForHost);

            response.Append(" :Your host isn't among the privileged");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 464
        /// </summary>
        /// <param name="info"></param>
        public void SendPasswordMismatch(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorPasswordMismatch);

            response.Append(" :Password incorrect");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 465
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreBannedCreep(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorYouAreBannedCreep);

            response.Append(" :You are banned from this server");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 466
        /// </summary>
        /// <param name="info"></param>
        public void SendYouWillBeBanned(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorYouWillBeBanned);

            response.Append(" :You will be banned from this server");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 471
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelIsFull(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorChannelIsFull);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :Cannot join channel (+l)");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 472
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mode"></param>
        public void SendUnknownMode(UserInfo info, ChannelInfo chan, char mode)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUnknownMode);

            response.Append(" ");
            response.Append(mode);
            response.Append(" :is unknown mode char to me for ");
            response.Append(chan.Name);

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 473
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendInviteOnlyChannel(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorInviteOnlyChannel);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :Cannot join channel (+i)");

            info.WriteLine(response);
        }
        /// <summary>
        /// Reply Code 474
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBannedFromChannel(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBannedFromChannel);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :Cannot join channel (+b)");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 475
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBadChannelKey(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBadChannelKey);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :Cannot join channel (+k)");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 476
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBadChannelMask(UserInfo info, string chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBadChannelMask);

            response.Append(" ");
            response.Append(chan);
            response.Append(" :Bad Channel Mask");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 477
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNoChannelModes(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoChannelModes);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :Channel doesn't support modes");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 478
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        /// <param name="mode"></param>
        public void SendBanListFull(UserInfo info, ChannelInfo chan, char mode)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBanListFull);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" ");
            response.Append(mode);
            response.Append(" :Channel list is full");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 480
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        public void SendCannotKnock(UserInfo info, string channel, string reason)
        {
            BuildMessageHeader(info, ReplyCode.ErrorCannotKnock);

            response.Append(" :Cannot knock on ");
            response.Append(channel);

            response.Append(" (");
            response.Append(reason);
            response.Append(")");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 481
        /// </summary>
        /// <param name="info"></param>
        public void SendNoPrivileges(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoPrivileges);

            response.Append(" :Permission Denied- You're not an IRC operator");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 482
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelOpPrivilegesNeeded(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorChannelOpPrivilegesNeeded);

            response.Append(" ");
            response.Append(chan.Name);
            response.Append(" :You're not channel operator");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 483
        /// </summary>
        /// <param name="info"></param>
        public void SendCannotKillServer(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorCannotKillServer);

            response.Append(" :You can't kill a server!");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 484
        /// </summary>
        /// <param name="info"></param>
        public void SendRestricted(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorRestricted);

            response.Append(" :Your connection is restricted!");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 485
        /// </summary>
        /// <param name="info"></param>
        public void SendUniqueOpPrivilegesNeeded(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUniqueOpPrivilegesNeeded);

            response.Append(" :You're not the original channel operator");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply 491
        /// </summary>
        /// <param name="info"></param>
        public void SendNoOperHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoOperHost);

            response.Append(" :No O-lines for your host");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 501
        /// </summary>
        /// <param name="info"></param>
        public void SendUserModeUnknownFlag(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUserModeUnknownFlag);

            response.Append(" :Unknown MODE flag");

            info.WriteLine(response);
        }

        /// <summary>
        /// Reply Code 502
        /// </summary>
        /// <param name="info"></param>
        public void SendUsersDoNotMatch(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUsersDoNotMatch);

            response.Append(" :Cannot change mode for other users");

            info.WriteLine(response);
        }

    }
}
