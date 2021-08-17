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
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class PrivateMessage : CommandBase
    {
        public PrivateMessage(IrcDaemon ircDaemon)
            : base(ircDaemon, "PRIVMSG", "P")
        { }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNoRecipient(info, Name);
                return;
            }

            if (args.Count < 2)
            {
                IrcDaemon.Replies.SendNoTextToSend(info);
                return;
            }

            // Only Private Messages set this
            info.LastAction = DateTime.Now;

            if (IrcDaemon.ValidChannel(args[0]))
            {
                if (IrcDaemon.Channels.ContainsKey(args[0]))
                {
                    var chan = IrcDaemon.Channels[args[0]];

                    if (!chan.Modes.HandleEvent(this, chan, info, args))
                    {
                        return;
                    }

                    // Send Channel Message
                    Send(new PrivateMessageArgument(info, chan, chan.Name, args[1]));
                }
                else
                {
                    IrcDaemon.Replies.SendCannotSendToChannel(info, args[0]);
                }
            }
            else if (IrcDaemon.ValidNick(args[0]))
            {
                if (IrcDaemon.Nicks.TryGetValue(args[0], out UserInfo user))
                {
                    if (user.Modes.Exist<ModeAway>())
                    {
                        IrcDaemon.Replies.SendAwayMessage(info, user);
                    }

                    // Send Private Message
                    Send(new PrivateMessageArgument(info, user, user.Nick, args[1]));
                }
                else
                {
                    IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
                }
            }
            else
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<PrivateMessageArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.Target);
            Command.Append(" :");
            Command.Append(arg.Message);

            return arg.Receiver.WriteLine(Command, arg.Sender);
        }
    }
}