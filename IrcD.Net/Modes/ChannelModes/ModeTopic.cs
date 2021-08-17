﻿/*
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
    public class ModeTopic : ChannelMode
    {
        public ModeTopic()
            : base('t')
        {
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (command is Topic)
            {
                if (!channel.UserPerChannelInfos.TryGetValue(user.Nick, out UserPerChannelInfo upci))
                {
                    user.IrcDaemon.Replies.SendNotOnChannel(user, channel.Name);
                    return false;
                }

                if (upci.Modes.Level < 30)
                {
                    user.IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(user, channel);
                    return false;
                }
            }

            return true;
        }
    }
}