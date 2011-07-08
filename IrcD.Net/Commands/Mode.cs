/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
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
using IrcD.Commands.Arguments;

namespace IrcD.Commands
{
    public class Mode : CommandBase
    {
        public Mode(IrcDaemon ircDaemon)
            : base(ircDaemon, "MODE")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            // Check if its a channel
            if (IrcDaemon.ValidChannel(args[0]))
            {

                if (!IrcDaemon.Channels.ContainsKey(args[0]))
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                    return;
                }
                var chan = IrcDaemon.Channels[args[0]];

                // Modes command without any mode -> query the Mode of the Channel
                if (args.Count == 1)
                {
                    IrcDaemon.Replies.SendChannelModeIs(info, chan);
                    return;
                }

                // Update the Channel Modes
                chan.Modes.Update(info, chan, args.Skip(1));
            }
            else if (args[0] == info.Nick)
            {
                // Modes command without any mode -> query the Mode of the User
                if (args.Count == 1)
                {
                    IrcDaemon.Replies.SendUserModeIs(info);
                    return;
                }

                // Update the User Modes
                info.Modes.Update(info, args.Skip(1));
            }
            else
            {
                // You cannot use Mode on any user but yourself
                IrcDaemon.Replies.SendUsersDoNotMatch(info);
            }
        }

        protected override void PrivateSend(CommandArgument commandArgument)
        {
            var arg = commandArgument as ModeArgument;

            BuildMessageHeader(arg);

            command.Append(arg.Target);
            command.Append(" ");
            command.Append(arg.ModeString);

            arg.Receiver.WriteLine(command);
        }
    }
}