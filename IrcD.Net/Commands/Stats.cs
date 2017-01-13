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

using System;
using System.Collections.Generic;
using System.Linq;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class Stats : CommandBase
    {
        public Stats(IrcDaemon ircDaemon)
            : base(ircDaemon, "STATS", "R")
        { }

        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendEndOfStats(info, "-");
            }

            //ToDo: parse target parameter

            if (args[0].Any(l => l == 'l'))
            {
                IrcDaemon.Replies.SendStatsLinkInfo(info);
            }
            if (args[0].Any(l => l == 'm'))
            {
                foreach (var command in IrcDaemon.Commands.OrderBy(c => c.Name))
                {
                    IrcDaemon.Replies.SendStatsCommands(info, command);
                }
            }
            if (args[0].Any(l => l == 'o'))
            {
                foreach (var op in IrcDaemon.Nicks
                    .Select(u => u.Value)
                    .Where(n => n.Modes.Exist<ModeOperator>() || n.Modes.Exist<ModeLocalOperator>())
                    .OrderBy(o => o.Nick))
                {
                    IrcDaemon.Replies.SendStatsOLine(info, op);
                }

            }

            if (args[0].Any(l => l == 'u'))
            {
                IrcDaemon.Replies.SendStatsUptime(info);
            }

            IrcDaemon.Replies.SendEndOfStats(info, args[0]);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}
