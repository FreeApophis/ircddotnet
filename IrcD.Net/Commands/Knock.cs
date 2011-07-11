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
using System.Linq;
using IrcD.Channel;
using IrcD.Commands.Arguments;

namespace IrcD.Commands
{
    public class Knock : CommandBase
    {
        public Knock(IrcDaemon ircDaemon)
            : base(ircDaemon, "KNOCK")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            ChannelInfo chan;

            if (IrcDaemon.Channels.TryGetValue(args[0], out chan))
            {
                if (!chan.Modes.HandleEvent(this, chan, info, args))
                {
                    return;
                }

                Send(new NoticeArgument(chan, chan.Name, "[KNOCK] by " + info.Usermask + "(" + ((args.Count > 1) ? args[1] : "no reason specified") + ")"));
                Send(new NoticeArgument(info, info.Nick, "Knocked on " + chan.Name));
            }
            else
            {
                IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
            }

        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = commandArgument as KnockArgument;

            BuildMessageHeader(arg);

            command.Append(arg.Channel.Name);
            command.Append(" :");
            command.Append(arg.Message);

            return arg.Receiver.WriteLine(command);
        }

        public override IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Repeat("KNOCK", 1);
        }
    }
}
