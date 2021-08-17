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
using IrcD.Channel;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Knock : CommandBase
    {
        public Knock(IrcDaemon ircDaemon)
            : base(ircDaemon, "KNOCK", "")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {

            if (IrcDaemon.Channels.TryGetValue(args[0], out ChannelInfo chan))
            {
                if (!chan.Modes.HandleEvent(this, chan, info, args))
                {
                    return;
                }

                Send(new NoticeArgument(chan, chan.Name, "[KNOCK] by " + info.Usermask + "(" + ((args.Count > 1) ? args[1] : "no reason specified") + ")"));
                Send(new NoticeArgument(info, info.Nick, "Knocked on " + chan.Name));
            }
            else
            {
                IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
            }

        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<KnockArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.Channel.Name);
            Command.Append(" :");
            Command.Append(arg.Message);

            return arg.Receiver.WriteLine(Command);
        }

        public override IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Repeat("KNOCK", 1);
        }
    }
}
