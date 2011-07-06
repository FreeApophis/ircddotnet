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
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class User : CommandBase
    {
        public User(IrcDaemon ircDaemon)
            : base(ircDaemon, "USER")
        { }

        [CheckParamCount(4)]
        public override void Handle(UserInfo info, List<string> args)
        {

            if (!info.PassAccepted)
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
                return;
            }
            if (info.UserExists)
            {
                IrcDaemon.Replies.SendAlreadyRegistered(info);
                return;
            }

            int flags;
            int.TryParse(args[1], out flags);

            if ((flags & 8) > 0)
            {
                info.Modes.Add(new ModeInvisible());
            }
            if ((flags & 4) > 0)
            {
                info.Modes.Add(new ModeWallops());
            }

            info.InitUser(args[0], args[3]);
        }
    }
}
