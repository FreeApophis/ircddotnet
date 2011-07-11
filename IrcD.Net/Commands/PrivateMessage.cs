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

using System;
using System.Collections.Generic;
using IrcD.Commands.Arguments;
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class PrivateMessage : CommandBase
    {
        public PrivateMessage(IrcDaemon ircDaemon)
            : base(ircDaemon, "PRIVMSG")
        { }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNoRecipient(info, Name);
                return;
            }
            if (args.Count < 2)
            {
                IrcDaemon.Replies.SendNoTextToSend(info);
                return;
            }

            // Only Private Messages set this
            info.LastAction = DateTime.Now;

            if (IrcDaemon.ValidChannel(args[0]))
            {
                if (IrcDaemon.Channels.ContainsKey(args[0]))
                {
                    var chan = IrcDaemon.Channels[args[0]];

                    if (!chan.Modes.HandleEvent(this, chan, info, args))
                    {
                        return;
                    }

                    // Send Channel Message
                    Send(new PrivateMessageArgument(info, chan, chan.Name, args[1]));
                }
                else
                {
                    IrcDaemon.Replies.SendCannotSendToChannel(info, args[0]);
                }
            }
            else if (IrcDaemon.ValidNick(args[0]))
            {
                UserInfo user;
                if (IrcDaemon.Nicks.TryGetValue(args[0], out user))
                {
                    if (user.Modes.Exist<ModeAway>())
                    {
                        IrcDaemon.Replies.SendAwayMessage(info, user);
                    }

                    // Send Private Message
                    Send(new PrivateMessageArgument(info, user, user.Nick, args[1]));
                }
                else
                {
                    IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
                }
            }
            else
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = commandArgument as PrivateMessageArgument;

            BuildMessageHeader(arg);

            command.Append(arg.Target);
            command.Append(" :");
            command.Append(arg.Message);

            return arg.Receiver.WriteLine(command, arg.Sender);
        }
    }
}