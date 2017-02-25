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
using IrcD.Commands.Arguments;
using IrcD.Core;

namespace IrcD.Commands
{
    public class Nick : CommandBase
    {
        public Nick(IrcDaemon ircDaemon)
            : base(ircDaemon, "NICK", "N")
        { }

        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (!info.PassAccepted)
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
                return;
            }

            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNoNicknameGiven(info);
                return;
            }

            if (IrcDaemon.Nicks.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNicknameInUse(info, args[0]);
                return;
            }

            if (!IrcDaemon.ValidNick(args[0]))
            {
                IrcDaemon.Replies.SendErroneousNickname(info, args[0]);
                return;
            }

            // *** NICK command valid after this point ***

            if (!info.NickExists)
            {
                //First Nick Command
                IrcDaemon.Nicks.Add(args[0], info);
                info.InitNick(args[0]);
                return;
            }

            // Announce nick change to itself

            Send(new NickArgument(info, info, args[0]));

            // Announce nick change to all channels it is in
            foreach (var channelInfo in info.Channels)
            {
                Send(new NickArgument(info, channelInfo, args[0]));
            }

            info.Rename(args[0]);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<NickArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.NewNick);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
