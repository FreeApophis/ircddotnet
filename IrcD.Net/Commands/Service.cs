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

namespace IrcD.Commands
{
    public class Service : CommandBase
    {
        public Service(IrcDaemon ircDaemon)
            : base(ircDaemon, "SERVICE", "")
        { }

        [CheckParamCount(6)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (info.Registered)
            {
                IrcDaemon.Replies.SendAlreadyRegistered(info);
                return;
            }

            if (!IrcDaemon.ValidNick(args[0]))
            {
                IrcDaemon.Replies.SendErroneousNickname(info, args[0]);
                return;
            }

            if (IrcDaemon.Nicks.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNicknameInUse(info, args[0]);
                return;
            }

            info.IsService = true;
            info.InitNick(args[0]);
            info.InitUser("service", "I am a service");

            IrcDaemon.Nicks.Add(info.Nick, info);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}