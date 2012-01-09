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

namespace IrcD.Commands
{
    public class Pong : CommandBase
    {
        public Pong(IrcDaemon ircDaemon)
            : base(ircDaemon, "PONG", "Z")
        { }

        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<PongArgument>(commandArgument);

            Command.Length = 0;
            Command.Append(IrcDaemon.ServerPrefix);
            Command.Append(" ");
            Command.Append(arg.Name);
            Command.Append(" ");
            Command.Append(IrcDaemon.ServerPrefix);
            Command.Append(" ");
            Command.Append(arg.Parameter);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
