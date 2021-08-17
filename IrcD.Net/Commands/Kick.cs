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
using IrcD.Modes.ChannelRanks;

namespace IrcD.Commands
{
    public class Kick : CommandBase
    {
        public Kick(IrcDaemon ircDaemon)
            : base(ircDaemon, "KICK", "K")
        { }

        [CheckRegistered]
        [CheckParamCount(2)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            var message = (args.Count > 2) ? args[2] : null;


            foreach (var subarg in GetSubArgument(args[0]).Zip(GetSubArgument(args[1]), (c, n) => new { Channel = c, Nick = n }))
            {

                if (!IrcDaemon.Channels.ContainsKey(subarg.Channel))
                {
                    IrcDaemon.Replies.SendNoSuchChannel(info, args[0]);
                    continue;
                }

                var chan = IrcDaemon.Channels[subarg.Channel];

                if (chan.UserPerChannelInfos.TryGetValue(info.Nick, out UserPerChannelInfo upci))
                {
                    if (upci.Modes.Level < ModeHalfOp.HalfOpLevel)
                    {
                        IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(info, chan);
                        continue;
                    }
                }
                else
                {
                    IrcDaemon.Replies.SendNotOnChannel(info, chan.Name);
                    continue;
                }

                if (chan.UserPerChannelInfos.TryGetValue(subarg.Nick, out UserPerChannelInfo kickUser))
                {
                    Send(new KickArgument(info, chan, chan, kickUser.UserInfo, message));
                    chan.RemoveUser(kickUser.UserInfo);
                }
                else
                {
                    IrcDaemon.Replies.SendUserNotInChannel(info, subarg.Channel, subarg.Nick);
                }
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            var arg = GetSaveArgument<KickArgument>(commandArgument);

            BuildMessageHeader(arg);

            Command.Append(arg.Channel.Name);
            Command.Append(" ");
            Command.Append(arg.User.Nick);
            Command.Append(" :");
            Command.Append(arg.Message ?? IrcDaemon.Options.StandardKickMessage);

            return arg.Receiver.WriteLine(Command);
        }
    }
}
