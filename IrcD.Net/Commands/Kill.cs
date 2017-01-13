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
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class Kill : CommandBase
    {
        public Kill(IrcDaemon ircDaemon)
            : base(ircDaemon, "KILL", "D")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (!info.Modes.Exist<ModeOperator>() && !info.Modes.Exist<ModeLocalOperator>())
            {
                IrcDaemon.Replies.SendNoPrivileges(info);
                return;
            }

            UserInfo killUser;
            if (!IrcDaemon.Nicks.TryGetValue(args[0], out killUser))
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
            }

            var message = args.Count > 1 ? args[1] : IrcDaemon.Options.StandardKillMessage;

            Send(new KillArgument(info, killUser, message));
            killUser?.Remove(IrcDaemon.Options.StandardKillMessage);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<KillArgument>(commandArgument);

            BuildMessageHeader(arg);
            var user = arg.Receiver as UserInfo;
            Command.Append((user != null) ? user.Nick : "nobody");
            Command.Append(" :");
            Command.Append(arg.Message);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
