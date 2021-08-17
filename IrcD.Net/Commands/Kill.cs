﻿/*
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

            if (!IrcDaemon.Nicks.TryGetValue(args[0], out UserInfo killUser))
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
            Command.Append((arg.Receiver is UserInfo user) ? user.Nick : "nobody");
            Command.Append(" :");
            Command.Append(arg.Message);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
