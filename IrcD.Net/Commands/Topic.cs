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
using IrcD.Core.Utils;

namespace IrcD.Commands
{
    public class Topic : CommandBase
    {
        public Topic(IrcDaemon ircDaemon)
            : base(ircDaemon, "TOPIC", "T")
        { }

        [CheckRegistered]
        [CheckParamCount(1)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (!IrcDaemon.Channels.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                return;
            }

            var chan = IrcDaemon.Channels[args[0]];

            if (args.Count == 1)
            {
                if (string.IsNullOrEmpty(chan.Topic))
                {
                    IrcDaemon.Replies.SendNoTopicReply(info, chan);
                }
                else
                {
                    IrcDaemon.Replies.SendTopicReply(info, chan);
                }
                return;
            }

            chan.Topic = args[1];

            // Some Mode might want to handle the command
            if (!chan.Modes.HandleEvent(this, chan, info, args))
            {
                return;
            }

            foreach (var user in chan.Users)
            {
                Send(new TopicArgument(info, user, chan, chan.Topic));
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<TopicArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.Channel.Name);
            Command.Append(" :");
            Command.Append(arg.NewTopic);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
