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
using System.Text;
using IrcD.Commands;
using IrcD.Commands.Arguments;

namespace IrcD.Modes
{
    public class UserModeList : ModeList<UserMode>
    {
        public UserModeList(IrcDaemon ircDaemon)
            : base(ircDaemon)
        {

        }

        public bool HandleEvent(CommandBase command, UserInfo user, List<string> args)
        {
            return Values.All(mode => mode.HandleEvent(command, user, args));
        }

        internal void Update(UserInfo info, IEnumerable<string> args)
        {
            var plus = true;
            var lastprefix = ' ';
            var validmode = new StringBuilder();

            foreach (var modechar in args.First())
            {
                if (modechar == '+' || modechar == '-')
                {
                    plus = (modechar == '+');
                    continue;
                }

                var umode = IrcDaemon.ModeFactory.GetUserMode(modechar);
                if (umode == null) continue;
                if (plus)
                {
                    if (!ContainsKey(umode.Char))
                    {
                        Add(umode);

                        if (lastprefix != '+')
                        {
                            validmode.Append(lastprefix = '+');
                        }
                        validmode.Append(umode.Char);
                    }
                }
                else
                {
                    if (ContainsKey(umode.Char))
                    {
                        Remove(umode.Char);

                        if (lastprefix != '-')
                        {
                            validmode.Append(lastprefix = '-');
                        }
                        validmode.Append(umode.Char);
                    }
                }
            }

            info.IrcDaemon.Commands.Send(new ModeArgument(info, info, info.Nick, validmode.ToString()));
        }

        public string ToUserModeString()
        {
            return "+" + ToString();
        }
    }
}