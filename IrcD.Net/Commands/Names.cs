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
    public class Names : CommandBase
    {
        public Names(IrcDaemon ircDaemon)
            : base(ircDaemon, "NAMES", "E")
        { }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (args.Count < 1)
            {
                // TODO: list all visible users
                return;
            }

            //TODO: taget parameter
            foreach (var ch in GetSubArgument(args[0]).Where(ch => IrcDaemon.Channels.ContainsKey(ch)))
            {
                IrcDaemon.Replies.SendNamesReply(info, IrcDaemon.Channels[ch]);
                IrcDaemon.Replies.SendEndOfNamesReply(info, IrcDaemon.Channels[ch]);
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}