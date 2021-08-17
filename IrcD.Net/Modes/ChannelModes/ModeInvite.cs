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
using IrcD.Commands;
using IrcD.Core;

namespace IrcD.Modes.ChannelModes
{
    public class ModeInvite : ChannelMode
    {
        public ModeInvite()
            : base('i')
        {
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (command is Join)
            {
                if (!user.Invited.Contains(channel))
                {
                    user.IrcDaemon.Replies.SendInviteOnlyChannel(user, channel);
                    return false;
                }

                user.Invited.Remove(channel);
            }

            if (command is Invite)
            {
                if (channel.UserPerChannelInfos.TryGetValue(user.Nick, out UserPerChannelInfo upci))
                {
                    if (upci.Modes.Level < 30)
                    {
                        channel.IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(user, channel);
                        return false;
                    }
                }
                else
                {
                    channel.IrcDaemon.Replies.SendNotOnChannel(user, channel.Name);
                    return false;
                }
            }

            return true;
        }
    }
}
