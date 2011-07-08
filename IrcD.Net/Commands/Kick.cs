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
using IrcD.Commands.Arguments;
using IrcD.Utils;

namespace IrcD.Commands
{
    public class Kick : CommandBase
    {
        public Kick(IrcDaemon ircDaemon)
            : base(ircDaemon, "KICK")
        { }

        [CheckRegistered]
        [CheckParamCount(2)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            var message = (args.Count > 2) ? args[2] : null;


            foreach (var subarg in GetSubArgument(args[0]).Zip(GetSubArgument(args[1]), (c, n) => new { Channel = c, Nick = n }))
            {

                if (!IrcDaemon.Channels.ContainsKey(subarg.Channel))
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                    continue;
                }

                var chan = IrcDaemon.Channels[subarg.Channel];
                UserPerChannelInfo upci;

                if (chan.UserPerChannelInfos.TryGetValue(info.Nick, out upci))
                {
                    if (upci.Modes.Level < 30)
                    {
                        IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(info, chan);
                        continue;
                    }
                }
                else
                {
                    IrcDaemon.Replies.SendNotOnChannel(info, chan.Name);
                    continue;
                }

                UserPerChannelInfo kickUser;
                if (chan.UserPerChannelInfos.TryGetValue(subarg.Nick, out kickUser))
                {
                    Send(new KickArgument(info, chan, chan, kickUser.UserInfo, message));

                    chan.UserPerChannelInfos.Remove(kickUser.UserInfo.Nick);
                    kickUser.UserInfo.UserPerChannelInfos.Remove(kickUser);

                }
                else
                {
                    IrcDaemon.Replies.SendUserNotInChannel(info, subarg.Channel, subarg.Nick);
                }
            }
        }

        protected override void PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<KickArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.Channel.Name);
            Command.Append(" ");
            Command.Append(arg.User.Nick);
            Command.Append(" :");
            Command.Append(arg.Message ?? IrcDaemon.Options.StandardKickMessage);

            arg.Receiver.WriteLine(Command);
        }
    }
}
