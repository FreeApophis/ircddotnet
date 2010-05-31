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
    public class Topic : CommandBase
    {
        public Topic(IrcDaemon ircDaemon)
            : base(ircDaemon, "TOPIC")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {

        }
    }
}

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