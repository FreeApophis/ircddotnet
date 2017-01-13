/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2017 Thomas Bruderer <apophis@apophis.ch>
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
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Part : CommandBase
    {
        public Part(IrcDaemon ircDaemon)
            : base(ircDaemon, "PART", "L")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            var message = (args.Count > 1) ? args[1] : IrcDaemon.Options.StandardPartMessage;


            foreach (string channelName in GetSubArgument(args[0]))
            {
                if (IrcDaemon.Channels.ContainsKey(channelName))
                {
                    var chan = IrcDaemon.Channels[channelName];

                    if (info.Channels.Contains(chan))
                    {
                        Send(new PartArgument(info, chan, chan, message));
                        chan.RemoveUser(info);
                    }
                    else
                    {
                        IrcDaemon.Replies.SendNotOnChannel(info, channelName);
                    }
                }
                else
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, channelName);
                }
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<PartArgument>(commandArgument);


            BuildMessageHeader(arg);

            Command.Append(arg.Channel.Name);
            Command.Append(" :");
            Command.Append(arg.Message);

            return arg.Receiver.WriteLine(Command);

        }
    }
}

