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

using System.Collections.Generic;
using System.Linq;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Ping : CommandBase
    {
        public Ping(IrcDaemon ircDaemon)
            : base(ircDaemon, "PING", "G")
        { }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            Send(new PongArgument(info, args.FirstOrDefault()));
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<PingArgument>(commandArgument);


            Command.Length = 0;
            Command.Append("PING ");
            Command.Append(IrcDaemon.ServerPrefix);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
