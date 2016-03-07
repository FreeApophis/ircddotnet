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
using IrcD.Commands;

namespace IrcD.ServerReplies
{
    public class ServerReplies
    {
        private const string NumericFormat = "{0:000}";
        private readonly StringBuilder _response = new StringBuilder();
        private readonly IrcDaemon _ircDaemon;

        public ServerReplies(IrcDaemon ircDaemon)
        {
            _ircDaemon = ircDaemon;
        }

        #region Helper Methods
        public void RegisterComplete(UserInfo info)
        {
            SendWelcome(info);
            SendYourHost(info);
            SendCreated(info);
            SendMyInfo(info);
            SendISupport(info);
            if (string.IsNullOrEmpty(_ircDaemon.Options.MessageOfTheDay))
            {
                SendNoMotd(info);
            }
            else
            {
                SendMotdStart(info);
                SendMotd(info);
                SendMotdEnd(info);
            }

            if (_ircDaemon.Options.IrcMode == IrcMode.Modern)
            {
                SendListUserClient(info);
                SendListUserOp(info);
                SendListUserUnknown(info);
                SendListUserChannels(info);
                SendListUserMe(info);
            }
        }

        private void BuildMessageHeader(UserInfo info, ReplyCode code)
        {
            _response.Length = 0;
            _response.Append(_ircDaemon.ServerPrefix);
            _response.Append(" ");
            _response.AppendFormat(NumericFormat, (int)code);
            _response.Append(" ");
            _response.Append(info.Nick ?? "-");
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
            var postfixlength = 2 + postfix?.Length ?? 0;

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

            _response.Append(" :Welcome to the ");
            _response.Append(info.IrcDaemon.Options.NetworkName);
            _response.Append(" IRC Network ");
            _response.Append(info.Usermask);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 002
        /// </summary>
        /// <param name="info"></param>
        public void SendYourHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YourHost);

            _response.Append(" :Your host is ");
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(", running version ");
            _response.Append(System.Reflection.Assembly.GetExecutingAssembly().FullName);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 003
        /// </summary>
        /// <param name="info"></param>
        public void SendCreated(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Created);

            _response.Append(" :This server was created ");
            _response.Append(_ircDaemon.ServerCreated);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 004
        /// </summary>
        /// <param name="info"></param>
        public void SendMyInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.MyInfo);

            _response.Append(" :");
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(" ");
            _response.Append("ircD.Net.");
            _response.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            _response.Append(" ");
            _response.Append(_ircDaemon.SupportedUserModes);
            _response.Append(" ");
            _response.Append(_ircDaemon.SupportedRanks.ToString() + _ircDaemon.SupportedChannelModes);

            info.WriteLine(_response);
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
            _response.Append(" ");

            SendSplitted(_response.ToString(), info, features, "are supported by this server");
        }

        /// <summary>
        /// Reply Code 005 / 010
        /// </summary>
        /// <param name="info"></param>
        public void SendBounce(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Bounce);

            // TODO: bounce to which server
            _response.Append(" :Try server <server name>, port <port number>");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 211
        /// </summary>
        /// <param name="info"></param>
        public void SendStatsLinkInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.StatsLinkInfo);
            //ToDo: <linkname> <sendq> <sent messages> <sent Kbytes> <received messages> <received Kbytes> <time open>

            _response.Append(" ");
            _response.Append(info.Socket);
            _response.Append(" ");
            _response.Append("0");
            _response.Append(" ");
            _response.Append("0");
            _response.Append(" ");
            _response.Append("0");
            _response.Append(" ");
            _response.Append("0");
            _response.Append(" ");
            _response.Append("0");
            _response.Append(" ");
            _response.Append((long)(DateTime.Now - info.Created).TotalSeconds);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 212
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public void SendStatsCommands(UserInfo info, CommandBase command)
        {
            BuildMessageHeader(info, ReplyCode.StatsCommands);

            _response.Append(" ");
            _response.Append(command.Name);
            _response.Append(" ");
            _response.Append(command.CallCountIn);
            _response.Append(" ");
            _response.Append(command.ByteCountIn + command.ByteCountOut);
            _response.Append(" ");
            _response.Append(command.CallCountOut);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 219
        /// </summary>
        /// <param name="info"></param>
        /// <param name="query"></param>
        public void SendEndOfStats(UserInfo info, string query)
        {
            BuildMessageHeader(info, ReplyCode.EndOfStats);

            _response.Append(" ");
            _response.Append(query);
            _response.Append(" :End of STATS report");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 242
        /// </summary>
        /// <param name="info"></param>
        public void SendStatsUptime(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.StatsUptime);

            _response.Append(" :Server Up ");
            _response.Append(_ircDaemon.Stats.Uptime.Days);
            _response.Append(" days ");
            _response.Append($"{_ircDaemon.Stats.Uptime.Hours:00}");
            _response.Append(":");
            _response.Append($"{_ircDaemon.Stats.Uptime.Minutes:00}");
            _response.Append(":");
            _response.Append($"{_ircDaemon.Stats.Uptime.Seconds:00}");

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply Code 243
        /// </summary>
        /// <param name="info"></param>
        /// <param name="op"></param>
        public void SendStatsOLine(UserInfo info, UserInfo op)
        {
            BuildMessageHeader(info, ReplyCode.StatsOLine);

            _response.Append(" O ");
            _response.Append(op.Host);
            _response.Append(" * ");
            _response.Append(op.Nick);

            info.WriteLine(_response);
        }



        /// <summary>
        /// Reply Code 221
        /// </summary>
        /// <param name="info"></param>
        public void SendUserModeIs(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.UserModeIs);

            _response.Append(" ");
            _response.Append(info.ModeString);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 251
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserClient(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserClient);

            _response.Append(" :There are ");
            _response.Append(_ircDaemon.Stats.UserCount);
            _response.Append(" users and ");
            _response.Append(_ircDaemon.Stats.ServiceCount);
            _response.Append(" services on ");
            _response.Append(_ircDaemon.Stats.ServerCount);
            _response.Append(" servers");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 252
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserOp(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserOp);

            _response.Append(_ircDaemon.Stats.OperatorCount);
            _response.Append(" :operator(s) online");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 253
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserUnknown(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserUnknown);

            _response.Append(_ircDaemon.Stats.UnknowConnectionCount);
            _response.Append(" :unknown connection(s)");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 254
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserChannels(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserChannels);

            _response.Append(_ircDaemon.Stats.ChannelCount);
            _response.Append(" :channels formed");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 255
        /// </summary>
        /// <param name="info"></param>
        public void SendListUserMe(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListUserMe);

            _response.Append(" :I have ");
            _response.Append(_ircDaemon.Stats.ClientCount);
            _response.Append(" clients and ");
            _response.Append(_ircDaemon.Stats.ServerCount);
            _response.Append(" servers");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 256
        /// </summary>
        /// <param name="info"></param>
        public void SendAdminMe(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.AdminMe);

            _response.Append(" ");
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(" :Administrative info");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 257
        /// </summary>
        /// <param name="info"></param>
        public void SendAdminLocation1(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.AdminLocation1);

            _response.Append(" :");
            _response.Append(_ircDaemon.Options.AdminLocation1);

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply Code 258
        /// </summary>
        /// <param name="info"></param>
        public void SendAdminLocation2(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.AdminLocation2);

            _response.Append(" :");
            _response.Append(_ircDaemon.Options.AdminLocation2);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 259
        /// </summary>
        /// <param name="info"></param>
        public void SendAdminEmail(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.AdminEmail);

            _response.Append(" :");
            _response.Append(_ircDaemon.Options.AdminEmail);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 301
        /// </summary>
        /// <param name="info"></param>
        /// <param name="awayUser"></param>
        public void SendAwayMessage(UserInfo info, UserInfo awayUser)
        {
            BuildMessageHeader(info, ReplyCode.Away);

            _response.Append(" ");
            _response.Append(awayUser.Nick);
            _response.Append(" :");
            _response.Append(awayUser.AwayMessage);

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply 302
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userInfos"></param>
        public void SendUserHost(UserInfo info, List<UserInfo> userInfos)
        {
            BuildMessageHeader(info, ReplyCode.UserHost);

            _response.Append(" :");

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

            SendSplitted(_response.ToString(), info, userHosts, null);
        }

        /// <summary>
        /// Reply 303
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nickList"></param>
        public void SendIsOn(UserInfo info, IEnumerable<string> nickList)
        {
            BuildMessageHeader(info, ReplyCode.IsOn);

            _response.Append(" :");

            SendSplitted(_response.ToString(), info, nickList, null);
        }

        /// <summary>
        /// Reply 305
        /// </summary>
        /// <param name="info"></param>
        public void SendUnAway(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.UnAway);

            _response.Append(" :You are no longer marked as being away");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 306
        /// </summary>
        /// <param name="info"></param>
        public void SendNowAway(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.NowAway);

            _response.Append(" :You have been marked as being away");

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply 311
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsUser(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsUser);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" ");
            _response.Append(who.User);
            _response.Append(" ");
            _response.Append(who.Host);
            _response.Append(" * :");
            _response.Append(who.RealName);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 312
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsServer(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsServer);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" ");
            _response.Append(_ircDaemon.Options.ServerName); // TODO: when doing multiple IRC Server
            _response.Append(" :");
            _response.Append(_ircDaemon.Options.NetworkName); // TODO: ServerInfo?

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 313
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsOperator(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsOperator);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" :is an IRC operator");

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply 315
        /// </summary>
        /// <param name="info"></param>
        /// <param name="mask"></param>
        public void SendEndOfWho(UserInfo info, string mask)
        {
            BuildMessageHeader(info, ReplyCode.EndOfWho);

            _response.Append(" ");
            _response.Append(mask);
            _response.Append(" :End of /WHO list.");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 317
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsIdle(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsIdle);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" ");
            if (_ircDaemon.Options.IrcMode == IrcMode.Modern)
            {
                _response.Append((int)((DateTime.Now - who.LastAction).TotalSeconds));
                _response.Append(" ");
                _response.Append(info.Created.ToUnixTime());
                _response.Append(" :seconds idle, signon time");

            }
            else
            {
                _response.Append((int)((DateTime.Now - who.LastAction).TotalSeconds));
                _response.Append(" :seconds idle");
            }

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 318
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendEndOfWhoIs(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.EndOfWhoIs);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" ");
            _response.Append(" :End of WHOIS list");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 319
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsChannels(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsChannels);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" :");

            SendSplitted(_response.ToString(), info, who.UserPerChannelInfos.Select(ucpi => ucpi.Modes.NickPrefix + ucpi.ChannelInfo.Name), null);
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

            _response.Append(" Channel :Users Name");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 322
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendListItem(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.List);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(chan.UserPerChannelInfos.Count);
            _response.Append(" :");
            _response.Append(chan.Topic);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 323
        /// </summary>
        /// <param name="info"></param>
        public void SendListEnd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ListEnd);

            _response.Append(" :End of LIST");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 324
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelModeIs(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ChannelModeIs);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(chan.ModeString);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 331
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNoTopicReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.NoTopic);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :No topic is set");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 332
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendTopicReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.Topic);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :");
            _response.Append(chan.Topic);

            info.WriteLine(_response);
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

            if (_ircDaemon.Options.IrcMode == IrcMode.Rfc1459 || _ircDaemon.Options.IrcMode == IrcMode.Rfc2810)
            {
                _response.Append(" ");
                _response.Append(channel);
            }

            _response.Append(" ");
            _response.Append(invited.Nick);

            if (_ircDaemon.Options.IrcMode == IrcMode.Modern)
            {
                _response.Append(" ");
                _response.Append(channel);
            }

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 342
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        public void SendSummoning(UserInfo info, string user)
        {
            BuildMessageHeader(info, ReplyCode.Summoning);

            _response.Append(" ");
            _response.Append(user);
            _response.Append(" :User summoned to irc");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(mask);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 347
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfInviteList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfInviteList);


            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :End of channel invite list");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(mask);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 349
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfExceptionList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfExceptionList);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :End of channel exception list");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 351
        /// </summary>
        /// <param name="info"></param>
        public void SendVersion(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Version);

            _response.Append(" ");
            _response.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            _response.Append(" ");
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(" :" + System.Reflection.Assembly.GetExecutingAssembly().FullName);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 352
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoReply(UserInfo info, UserPerChannelInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoReply);

            _response.Append(" ");
            _response.Append(who.ChannelInfo.Name);
            _response.Append(" ");
            _response.Append(who.UserInfo.User);
            _response.Append(" ");
            _response.Append(who.UserInfo.Host);
            _response.Append(" ");

            // TODO Server to Server
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(" ");
            _response.Append(who.UserInfo.Nick);
            _response.Append(" ");
            _response.Append(who.UserInfo.Modes.Exist<ModeAway>() ? "G" : "H");
            _response.Append(who.UserInfo.Modes.Exist<ModeOperator>() || who.UserInfo.Modes.Exist<ModeLocalOperator>() ? "*" : string.Empty);
            _response.Append(who.Modes.NickPrefix);
            _response.Append(" :");

            // TODO: Server to Server
            //response.Append(who.Server.Hops);            
            _response.Append(0);
            _response.Append(" ");
            _response.Append(who.UserInfo.RealName);

            //TODO: append d if deaf - add Deaf (such as Mode X / W on undernet)

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 353
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNamesReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.NamesReply);
            _response.Append(" ");
            _response.Append(chan.NamesPrefix);
            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :");

            SendSplitted(_response.ToString(), info, chan.UserPerChannelInfos.Values.Select(upci => upci.Modes.NickPrefix + upci.UserInfo.Nick), null);
        }

        /// <summary>
        /// Reply Code 366
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfNamesReply(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfNames);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :End of NAMES list");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(mask);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 368
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendEndOfBanList(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.EndOfBanList);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :End of channel ban list");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 371
        /// </summary>
        /// <param name="info"></param>
        public void SendInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Info);

            _response.Append(" :");
            _response.Append("TODO");

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply Code 372
        /// </summary>
        /// <param name="info"></param>
        public void SendMotd(UserInfo info)
        {
            foreach (var motdLine in _ircDaemon.Options.MessageOfTheDay.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                BuildMessageHeader(info, ReplyCode.Motd);

                _response.Append(" :- ");
                _response.Append(motdLine);

                info.WriteLine(_response);
            }
        }

        /// <summary>
        /// Reply Code 374
        /// </summary>
        /// <param name="info"></param>
        public void SendEndOfInfo(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.EndOfInfo);

            _response.Append(" :End of /INFO list");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 375
        /// </summary>
        /// <param name="info"></param>
        public void SendMotdStart(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.MotdStart);

            _response.Append(" :- ");
            _response.Append(_ircDaemon.Options.ServerName);
            _response.Append(" Message of the day -");

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply Code 376
        /// </summary>
        /// <param name="info"></param>
        public void SendMotdEnd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.EndOfMotd);

            _response.Append(" :End of MOTD command");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 381
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreOper(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YouAreOper);

            _response.Append(" :You are now an IRC operator");

            info.WriteLine(_response);
        }
        /// <summary>
        /// Reply Code 383
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreService(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YouAreService);

            _response.Append(" :You are service ");
            _response.Append(info.Nick);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 391
        /// </summary>
        /// <param name="info"></param>
        public void SendTimeReply(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Time);

            _response.Append(" ");
            _response.Append(_ircDaemon.ServerPrefix);
            _response.Append(" :");
            _response.Append(DateTime.Now);

            info.WriteLine(_response);
        }


        /// <summary>
        /// Reply Code 401
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendNoSuchNick(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchNickname);

            _response.Append(" ");
            _response.Append(nick);
            _response.Append(" :No such nick/channel");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 402
        /// </summary>
        /// <param name="info"></param>
        /// <param name="server"></param>
        public void SendNoSuchServer(UserInfo info, string server)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchServer);

            _response.Append(" ");
            _response.Append(server);
            _response.Append(" :No such server");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 403
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void SendNoSuchChannel(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoSuchChannel);

            _response.Append(" ");
            _response.Append(channel);
            _response.Append(" :No such channel");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(channel);
            _response.Append(" :");
            _response.Append(message);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 405
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void Send(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorTooManyChannels);

            _response.Append(" ");
            _response.Append(channel);
            _response.Append(" :You have joined too many channels");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 410
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        internal void SendInvalidCapabilitiesCommand(UserInfo info, string command)
        {
            BuildMessageHeader(info, ReplyCode.ErrorInvalidCapabilitesCommand);

            _response.Append(" ");
            _response.Append(command);
            _response.Append(" :Invalid CAP subcommand");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 411
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public void SendNoRecipient(UserInfo info, string command)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoRecipient);

            _response.Append(" :No recipient given (");
            _response.Append(command);
            _response.Append(")");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 412
        /// </summary>
        /// <param name="info"></param>
        public void SendNoTextToSend(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoTextToSend);

            _response.Append(" :No text to send");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 421
        /// </summary>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public void SendUnknownCommand(UserInfo info, string command)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUnknownCommand);

            _response.Append(" ");
            _response.Append(command);
            _response.Append(" :Unknown command");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 422
        /// </summary>
        /// <param name="info"></param>
        public void SendNoMotd(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoMotd);

            _response.Append(" :MOTD File is missing");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 431
        /// </summary>
        /// <param name="info"></param>
        public void SendNoNicknameGiven(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoNicknameGiven);

            _response.Append(" :No nickname given");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 432
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendErroneousNickname(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorErroneusNickname);

            _response.Append(" :Erroneous nickname");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 433
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nick"></param>
        public void SendNicknameInUse(UserInfo info, string nick)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNicknameInUse);

            _response.Append(" :Nickname is already in use");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(nick);
            _response.Append(" ");
            _response.Append(channel);
            _response.Append(" :They aren't on that channel");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 442
        /// </summary>
        /// <param name="info"></param>
        /// <param name="channel"></param>
        public void SendNotOnChannel(UserInfo info, string channel)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNotOnChannel);

            _response.Append(" ");
            _response.Append(channel);
            _response.Append(" :You're not on that channel");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(invited.Nick);
            _response.Append(" ");
            _response.Append(channel.Name);
            _response.Append(" :is already on channel");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 445
        /// </summary>
        /// <param name="info"></param>
        public void SendSummonDisabled(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorSummonDisabled);

            _response.Append(" :SUMMON has been disabled");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 446
        /// </summary>
        /// <param name="info"></param>
        public void SendUsersDisabled(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUsersDisabled);

            _response.Append(" :USERS has been disabled");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 451
        /// </summary>
        /// <param name="info"></param>
        public void SendNotRegistered(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNotRegistered);

            _response.Append(" :You have not registered");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Numeric Reply 461
        /// </summary>
        /// <param name="info"></param>
        public void SendNeedMoreParams(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNeedMoreParams);

            _response.Append(" :Not enough parameters");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Numeric Reply 462
        /// </summary>
        /// <param name="info"></param>
        public void SendAlreadyRegistered(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorAlreadyRegistered);

            _response.Append(" :Unauthorized command (already registered)");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Numeric Reply 463
        /// </summary>
        /// <param name="info"></param>
        public void SendNoPermissionForHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoPermissionForHost);

            _response.Append(" :Your host isn't among the privileged");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 464
        /// </summary>
        /// <param name="info"></param>
        public void SendPasswordMismatch(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorPasswordMismatch);

            _response.Append(" :Password incorrect");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 465
        /// </summary>
        /// <param name="info"></param>
        public void SendYouAreBannedCreep(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorYouAreBannedCreep);

            _response.Append(" :You are banned from this server");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 466
        /// </summary>
        /// <param name="info"></param>
        public void SendYouWillBeBanned(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorYouWillBeBanned);

            _response.Append(" :You will be banned from this server");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 471
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelIsFull(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorChannelIsFull);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :Cannot join channel (+l)");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(mode);
            _response.Append(" :is unknown mode char to me for ");
            _response.Append(chan.Name);

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 473
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendInviteOnlyChannel(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorInviteOnlyChannel);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :Cannot join channel (+i)");

            info.WriteLine(_response);
        }
        /// <summary>
        /// Reply Code 474
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBannedFromChannel(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBannedFromChannel);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :Cannot join channel (+b)");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 475
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBadChannelKey(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBadChannelKey);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :Cannot join channel (+k)");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 476
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendBadChannelMask(UserInfo info, string chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorBadChannelMask);

            _response.Append(" ");
            _response.Append(chan);
            _response.Append(" :Bad Channel Mask");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 477
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendNoChannelModes(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoChannelModes);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :Channel doesn't support modes");

            info.WriteLine(_response);
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

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" ");
            _response.Append(mode);
            _response.Append(" :Channel list is full");

            info.WriteLine(_response);
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

            _response.Append(" :Cannot knock on ");
            _response.Append(channel);

            _response.Append(" (");
            _response.Append(reason);
            _response.Append(")");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 481
        /// </summary>
        /// <param name="info"></param>
        public void SendNoPrivileges(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoPrivileges);

            _response.Append(" :Permission Denied- You're not an IRC operator");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 482
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chan"></param>
        public void SendChannelOpPrivilegesNeeded(UserInfo info, ChannelInfo chan)
        {
            BuildMessageHeader(info, ReplyCode.ErrorChannelOpPrivilegesNeeded);

            _response.Append(" ");
            _response.Append(chan.Name);
            _response.Append(" :You're not channel operator");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 483
        /// </summary>
        /// <param name="info"></param>
        public void SendCannotKillServer(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorCannotKillServer);

            _response.Append(" :You can't kill a server!");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 484
        /// </summary>
        /// <param name="info"></param>
        public void SendRestricted(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorRestricted);

            _response.Append(" :Your connection is restricted!");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 485
        /// </summary>
        /// <param name="info"></param>
        public void SendUniqueOpPrivilegesNeeded(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUniqueOpPrivilegesNeeded);

            _response.Append(" :You're not the original channel operator");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply 491
        /// </summary>
        /// <param name="info"></param>
        public void SendNoOperHost(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorNoOperHost);

            _response.Append(" :No O-lines for your host");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 501
        /// </summary>
        /// <param name="info"></param>
        public void SendUserModeUnknownFlag(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUserModeUnknownFlag);

            _response.Append(" :Unknown MODE flag");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 502
        /// </summary>
        /// <param name="info"></param>
        public void SendUsersDoNotMatch(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.ErrorUsersDoNotMatch);

            _response.Append(" :Cannot change mode for other users");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 687
        /// </summary>
        /// <param name="info"></param>
        public void SendYourLanguageIs(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.YourLanguageIs);

            _response.Append(info.Languages.Concatenate(","));
            _response.Append(" :Your languages have been set");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 688
        /// </summary>
        /// <param name="info"></param>
        public void SendLanguage(UserInfo info)
        {
            BuildMessageHeader(info, ReplyCode.Language);

            _response.Append(" <code> <revision> <maintainer> <flags> * :<info>");

            info.WriteLine(_response);
        }

        /// <summary>
        /// Reply Code 690
        /// </summary>
        /// <param name="info"></param>
        /// <param name="who"></param>
        public void SendWhoIsLanguage(UserInfo info, UserInfo who)
        {
            BuildMessageHeader(info, ReplyCode.WhoIsLanguage);

            _response.Append(" ");
            _response.Append(who.Nick);
            _response.Append(" ");
            _response.Append(who.Languages.Concatenate(","));
            _response.Append(" : ");
            _response.Append(who.Languages.Select(l => GoogleTranslate.Languages[l]).Concatenate(", "));

            info.WriteLine(_response);
        }
    }
}
