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
using IrcD.Modes.UserModes;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Away : CommandBase
    {
        public Away(IrcDaemon ircDaemon)
            : base(ircDaemon, "AWAY", "A")
        {
            if (!ircDaemon.Capabilities.Contains("away-notify"))
            {
                ircDaemon.Capabilities.Add("away-notify");
            }
        }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (args.Count == 0)
            {
                info.AwayMessage = null;
                info.Modes.RemoveMode<ModeAway>();
                IrcDaemon.Replies.SendUnAway(info);
            }
            else
            {
                info.AwayMessage = args[0];
                info.Modes.Add(new ModeAway());
                IrcDaemon.Replies.SendNowAway(info);
            }

            foreach (var channel in info.Channels)
            {
                foreach (var user in channel.Users)
                {
                    if (user.Capabilities.Contains("away-notify"))
                    {

                        Send(new AwayArgument(info, user, (args.Count == 0) ? null : args[0]));
                    }
                }
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<AwayArgument>(commandArgument);

            BuildMessageHeader(arg);

            if (arg.AwayMessage != null)
            {
                Command.Append(arg.AwayMessage);
            }

            return arg.Receiver.WriteLine(Command);
        }
    }
}
