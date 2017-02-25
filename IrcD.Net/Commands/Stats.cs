/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 *  
 * Copyright (c) 2009-2017, Thomas Bruderer, apophis@apophis.ch All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *   
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * * Neither the name of ArithmeticParser nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
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
