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
    public class Part : CommandBase
    {
        public Part(IrcDaemon ircDaemon)
            : base(ircDaemon, "PART")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
        }
    }
}

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
