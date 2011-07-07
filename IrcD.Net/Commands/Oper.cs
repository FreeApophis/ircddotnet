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
    public class Oper : CommandBase
    {
        public Oper(IrcDaemon ircDaemon)
            : base(ircDaemon, "OPER")
        { }

        [CheckRegistered]
        [CheckParamCount(2)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (DenyOper(info))
            {
                IrcDaemon.Replies.SendNoOperHost(info);
                return;
            }

            if (ValidOperLine(args[0], args[1]))
            {
                info.Modes.Add(new ModeLocalOperator());
                info.Modes.Add(new ModeOperator());

                IrcDaemon.Replies.SendYouAreOper(info);
            }
            else
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
            }
        }

        /// <summary>
        /// Check if an IRC Operatur status can be granted upon user and pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool ValidOperLine(string user, string pass)
        {
            string realpass;
            if (IrcDaemon.Options.OLine.TryGetValue(user, out realpass))
            {
                if (pass == realpass)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the Host of the user is allowed to use the OPER command
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool DenyOper(UserInfo info)
        {
            var allow = false;
            foreach (var operHost in IrcDaemon.Options.OperHosts)
            {
                if (!allow && operHost.Allow)
                    allow = operHost.WildcardHostMask.IsMatch(info.Host);

                if (allow && !operHost.Allow)
                    allow = !operHost.WildcardHostMask.IsMatch(info.Host);
            }
            return !allow;
        }
    }
}

