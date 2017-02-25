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
using IrcD.Channel;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Invite : CommandBase
    {
        public Invite(IrcDaemon ircDaemon)
            : base(ircDaemon, "INVITE", "I")
        { }

        [CheckRegistered]
        [CheckParamCount(2)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            UserInfo invited;
            if (!IrcDaemon.Nicks.TryGetValue(args[0], out invited))
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
            }

            var channel = args[1];
            ChannelInfo chan;
            if (IrcDaemon.Channels.TryGetValue(channel, out chan))
            {
                if (chan.UserPerChannelInfos.ContainsKey(invited.Nick))
                {
                    IrcDaemon.Replies.SendUserOnChannel(info, invited, chan);
                    return;
                }

                if (!chan.Modes.HandleEvent(this, chan, info, args))
                {
                    return;
                }

                if (!invited.Invited.Contains(chan))
                {
                    invited.Invited.Add(chan);
                }
            }

            //TODO channel does not exist? ... clean up below

            IrcDaemon.Replies.SendInviting(info, invited, channel);
            Send(new InviteArgument(info, invited, invited, chan));
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<InviteArgument>(commandArgument);


            BuildMessageHeader(arg);

            Command.Append(arg.Invited.Nick);
            Command.Append(" ");
            Command.Append(arg.Channel.Name);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
