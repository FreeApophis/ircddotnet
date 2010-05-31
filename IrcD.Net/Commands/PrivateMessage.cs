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

using System.Collections.Generic;

namespace IrcD.Commands
{
    public class PrivateMessage : CommandBase
    {
        public PrivateMessage(IrcDaemon ircDaemon)
            : base(ircDaemon, "PRIVMSG")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {

        }
    }
}

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