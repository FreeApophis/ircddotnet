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
    public class Mode : CommandBase
    {
        public Mode(IrcDaemon ircDaemon)
            : base(ircDaemon, "MODE", "M")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            // Check if its a channel
            if (IrcDaemon.ValidChannel(args[0]))
            {

                if (!IrcDaemon.Channels.ContainsKey(args[0]))
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                    return;
                }

                var chan = IrcDaemon.Channels[args[0]];

                // Modes command without any mode -> query the Mode of the Channel
                if (args.Count == 1)
                {
                    IrcDaemon.Replies.SendChannelModeIs(info, chan);
                    return;
                }

                // Update the Channel Modes
                chan.Modes.Update(info, chan, args.Skip(1));
            }
            else if (args[0] == info.Nick)
            {
                // Modes command without any mode -> query the Mode of the User
                if (args.Count == 1)
                {
                    IrcDaemon.Replies.SendUserModeIs(info);
                    return;
                }

                // Update the User Modes
                info.Modes.Update(info, args.Skip(1));
            }
            else
            {
                // You cannot use Mode on any user but yourself
                IrcDaemon.Replies.SendUsersDoNotMatch(info);
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<ModeArgument>(commandArgument);


            BuildMessageHeader(arg);

            Command.Append(arg.Target);
            Command.Append(" ");
            Command.Append(arg.ModeString);

            return arg.Receiver.WriteLine(Command);
        }
    }
}