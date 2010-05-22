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
//    class NumericErrorReplies : ServerReplies
//    {
//        #region Numeric Error Replies
//        /// <summary>
//        /// Reply Code 401
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="nick"></param>
//        protected void SendNoSuchNick(UserInfo info, string nick)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchNickname);
//            Response.Append(" ");
//            Response.Append(nick);
//            Response.Append(" :No such nick/channel");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 402
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="server"></param>
//        protected void SendNoSuchServer(UserInfo info, string server)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchServer);
//            Response.Append(" ");
//            Response.Append(server);
//            Response.Append(" :No such server");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 403
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="channel"></param>
//        protected void SendNoSuchChannel(UserInfo info, string channel)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoSuchChannel);
//            Response.Append(" ");
//            Response.Append(channel);
//            Response.Append(" :No such channel");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 404
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="channel"></param>
//        protected void SendCannotSendToChannel(UserInfo info, string channel)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorCannotSendToChannel);
//            Response.Append(" ");
//            Response.Append(channel);
//            Response.Append(" :Cannot send to channel");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 405
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="channel"></param>
//        protected void Send(UserInfo info, string channel)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorTooManyChannels);
//            Response.Append(" ");
//            Response.Append(channel);
//            Response.Append(" :You have joined too many channels");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 411
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="command"></param>
//        protected void SendNoRecipient(UserInfo info, string command)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoRecipient);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :No recipient given (");
//            Response.Append(command);
//            Response.Append(")");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 412
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoTextToSend(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoTextToSend);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :No text to send");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 421
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="command"></param>
//        protected void SendUnknownCommand(UserInfo info, string command)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUnknownCommand);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(command);
//            Response.Append(" :Unknown command");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 422
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoMotd(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoMotd);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :MOTD File is missing");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 431
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoNicknameGiven(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoNicknameGiven);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :No nickname given");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 432
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="nick"></param>
//        protected void SendErroneousNickname(UserInfo info, string nick)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorErroneusNickname);
//            Response.Append(" ");
//            Response.Append(nick);
//            Response.Append(" :Erroneous nickname");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 433
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="nick"></param>
//        protected void SendNicknameInUse(UserInfo info, string nick)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNicknameInUse);
//            Response.Append(" ");
//            Response.Append(nick);
//            Response.Append(" :Nickname is already in use");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 442
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="channel"></param>
//        protected void SendNotOnChannel(UserInfo info, string channel)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNotOnChannel);
//            Response.Append(" ");
//            Response.Append(channel);
//            Response.Append(" :You're not on that channel");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 445
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendSummonDisabled(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorSummonDisabled);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :SUMMON has been disabled");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 446
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendUsersDisabled(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUsersDisabled);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :USERS has been disabled");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 451
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNotRegistered(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNotRegistered);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You have not registered");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Numeric Reply 461
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNeedMoreParams(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNeedMoreParams);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Not enough parameters");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Numeric Reply 462
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendAlreadyRegistered(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorAlreadyRegistered);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Unauthorized command (already registered)");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Numeric Reply 463
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoPermissionForHost(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoPermissionForHost);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Your host isn't among the privileged");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 464
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendPasswordMismatch(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorPasswordMismatch);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Password incorrect");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 465
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendYouAreBannedCreep(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorYouAreBannedCreep);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You are banned from this server");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 466
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendYouWillBeBanned(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorYouWillBeBanned);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You will be banned from this server");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 471
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendChannelIsFull(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorChannelIsFull);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Cannot join channel (+l)");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 472
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        /// <param name="mode"></param>
//        protected void SendUnknownMode(UserInfo info, ChannelInfo chan, char mode)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUnknownMode);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(mode);
//            Response.Append(" :is unknown mode char to me for ");
//            Response.Append(chan.Name);
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 473
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendInviteOnlyChannel(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorInviteOnlyChannel);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Cannot join channel (+i)");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }
//        /// <summary>
//        /// Reply Code 474
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendBannedFromChannel(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBannedFromChannel);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Cannot join channel (+b)");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 475
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendBadChannelKey(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBadChannelKey);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Cannot join channel (+k)");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 476
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendBadChannelMask(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBadChannelMask);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Bad Channel Mask");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 477
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendNoChannelModes(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoChannelModes);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :Channel doesn't support modes");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 478
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        /// <param name="mode"></param>
//        protected void SendBanListFull(UserInfo info, ChannelInfo chan, char mode)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorBanListFull);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" ");
//            Response.Append(mode);
//            Response.Append(" :Channel list is full");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 481
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoPrivileges(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoPrivileges);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Permission Denied- You're not an IRC operator");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 482
//        /// </summary>
//        /// <param name="info"></param>
//        /// <param name="chan"></param>
//        protected void SendChannelOpPrivilegesNeeded(UserInfo info, ChannelInfo chan)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorChannelOpPrivilegesNeeded);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" ");
//            Response.Append(chan.Name);
//            Response.Append(" :You're not channel operator");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 483
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendCannotKillServer(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorCannotKillServer);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You can't kill a server!");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 484
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendRestricted(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorRestricted);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Your connection is restricted!");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 485
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendUniqueOpPrivilegesNeeded(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUniqueOpPrivilegesNeeded);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :You're not the original channel operator");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply 491
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendNoOperHost(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorNoOperHost);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :No O-lines for your host");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 501
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendUserModeUnknownFlag(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUserModeUnknownFlag);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Unknown MODE flag");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }

//        /// <summary>
//        /// Reply Code 502
//        /// </summary>
//        /// <param name="info"></param>
//        protected void SendUsersDoNotMatch(UserInfo info)
//        {
//            Response.Length = 0;
//            Response.Append(ServerPrefix);
//            Response.Append(" ");
//            Response.AppendFormat(NumericFormat, (int)ReplyCode.ErrorUsersDoNotMatch);
//            Response.Append(" ");
//            Response.Append(info.Nick);
//            Response.Append(" :Cannot change mode for other users");
//            Response.Append(ServerCrLf);
//            info.Socket.Send(Encoding.UTF8.GetBytes(Response.ToString()));
//        }
//        #endregion    }
//}
