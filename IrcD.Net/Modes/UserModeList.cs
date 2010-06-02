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
using System.Linq;
using IrcD.ServerReplies;

namespace IrcD.Modes
{
    public class UserModeList : ModeList<UserMode>
    {
        public bool HandleEvent(IrcCommandType ircCommand, UserInfo user, List<string> args)
        {
            return Values.All(mode => mode.HandleEvent(ircCommand, user, args));
        }

        internal void Update(UserInfo info, IEnumerable<string> args)
        {
            bool plus = true;
            foreach (var modechar in args.First())
            {
                if (modechar == '+' || modechar == '-')
                {
                    plus = (modechar == '+');
                    continue;
                }

                var cmode = ModeFactory.GetUserMode(modechar);
            }
        }

        public string ToUserModeString()
        {
            return "+" + ToString();
        }
    }
}