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
    public class Join : CommandBase
    {
        public Join(IrcDaemon ircDaemon)
            : base(ircDaemon, "JOIN")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
        }
    }
}

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