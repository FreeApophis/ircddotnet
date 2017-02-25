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
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class WhoIs : CommandBase
    {
        public WhoIs(IrcDaemon ircDaemon)
            : base(ircDaemon, "WHOIS", "W")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (!IrcDaemon.Nicks.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
                return;
            }

            var user = IrcDaemon.Nicks[args[0]];
            IrcDaemon.Replies.SendWhoIsUser(info, user);
            if (info.UserPerChannelInfos.Count > 0)
            {
                IrcDaemon.Replies.SendWhoIsChannels(info, user);
            }
            IrcDaemon.Replies.SendWhoIsServer(info, user);
            if (user.AwayMessage != null)
            {
                IrcDaemon.Replies.SendAwayMessage(info, user);
            }

            if (IrcDaemon.Options.IrcMode == IrcMode.Modern)
            {
                IrcDaemon.Replies.SendWhoIsLanguage(info, user);
            }


            if (user.Modes.Exist<ModeOperator>() || user.Modes.Exist<ModeLocalOperator>())
            {
                IrcDaemon.Replies.SendWhoIsOperator(info, user);
            }

            IrcDaemon.Replies.SendWhoIsIdle(info, user);
            IrcDaemon.Replies.SendEndOfWhoIs(info, user);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}
