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
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Pass : CommandBase
    {
        public Pass(IrcDaemon ircDaemon)
            : base(ircDaemon, "PASS", "PA")
        { }

        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (info.PassAccepted)
            {
                IrcDaemon.Replies.SendAlreadyRegistered(info);
                return;
            }

            if (args[0] == IrcDaemon.Options.ServerPass)
            {
                info.PassAccepted = true;
                return;
            }

            if (IrcDaemon.Options.ConnectionPasses.Any(p => p == args[0]))
            {
                // This is an allowed Server connection
                IrcDaemon.ConnectFromServer(info);
                return;
            }

            IrcDaemon.Replies.SendPasswordMismatch(info);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}
