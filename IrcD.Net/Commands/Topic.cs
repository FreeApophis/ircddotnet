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
using IrcD.ServerReplies;

namespace IrcD.Commands
{
    public class Topic : CommandBase
    {
        public Topic(IrcDaemon ircDaemon)
            : base(ircDaemon, "TOPIC")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                IrcDaemon.Replies.SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNeedMoreParams(info);
                return;
            }
            if (!IrcDaemon.Channels.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                return;
            }
            var chan = IrcDaemon.Channels[args[0]];

            if (args.Count == 1)
            {
                if (string.IsNullOrEmpty(chan.Topic))
                {
                    IrcDaemon.Replies.SendNoTopicReply(info, chan);
                }
                else
                {
                    IrcDaemon.Replies.SendTopicReply(info, chan);
                }
                return;
            }

            chan.Topic = args[1];

            if (!chan.Modes.HandleEvent(IrcCommandType.Topic, chan, info, args))
            {
                return;
            }

            foreach (var user in chan.Users)
            {
                IrcDaemon.Send.Topic(info, user, chan.Name, chan.Topic);
            }
        }
    }
}
