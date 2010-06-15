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
using IrcD.Channel;
using IrcD.ServerReplies;

namespace IrcD.Commands
{
    public class Invite : CommandBase
    {
        public Invite(IrcDaemon ircDaemon)
            : base(ircDaemon, "INVITE")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                IrcDaemon.Replies.SendNotRegistered(info);
                return;
            }
            if (args.Count < 2)
            {
                IrcDaemon.Replies.SendNeedMoreParams(info);
                return;
            }

            UserInfo invited;
            if (!IrcDaemon.Nicks.TryGetValue(args[0], out invited))
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
            }

            var channel = args[1];
            ChannelInfo chan;
            if (IrcDaemon.Channels.TryGetValue(channel, out chan))
            {
                if (chan.UserPerChannelInfos.ContainsKey(invited.Nick))
                {
                    IrcDaemon.Replies.SendUserOnChannel(info, invited, chan);
                    return;
                }

                if (!chan.Modes.HandleEvent(IrcCommandType.Invite, chan, info, args))
                {
                    return;
                }
                
                if (!invited.Invited.Contains(chan))
                {
                    invited.Invited.Add(chan);
                }
            }

            IrcDaemon.Replies.SendInviting(info, invited, channel);
            IrcDaemon.Send.Invite(info, invited, invited, channel);

        }
    }
}
