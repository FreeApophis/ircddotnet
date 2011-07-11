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
    public class Part : CommandBase
    {
        public Part(IrcDaemon ircDaemon)
            : base(ircDaemon, "PART")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            var message = (args.Count > 1) ? args[1] : IrcDaemon.Options.StandardPartMessage;


            foreach (string ch in GetSubArgument(args[0]))
            {
                if (IrcDaemon.Channels.ContainsKey(ch))
                {
                    var chan = IrcDaemon.Channels[ch];
                    var upci = chan.UserPerChannelInfos[info.Nick];

                    if (info.Channels.Contains(chan))
                    {
                        Send(new PartArgument(info, chan, chan, message));

                        chan.UserPerChannelInfos.Remove(info.Nick);
                        info.UserPerChannelInfos.Remove(upci);

                        if (!chan.UserPerChannelInfos.Any())
                        {
                            IrcDaemon.Channels.Remove(chan.Name);
                        }
                    }
                    else
                    {
                        IrcDaemon.Replies.SendNotOnChannel(info, ch);
                        continue;
                    }
                }
                else
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, ch);
                    continue;
                }
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = commandArgument as PartArgument;

            BuildMessageHeader(arg);

            command.Append(arg.Channel.Name);
            command.Append(" :");
            command.Append(arg.Message);

            return arg.Receiver.WriteLine(command);

        }
    }
}

