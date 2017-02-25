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
using IrcD.Commands;
using IrcD.Core;
using IrcD.Tools;

namespace IrcD.Modes.ChannelModes
{
    public class ModeColorless : ChannelMode
    {
        public ModeColorless()
            : base('c')
        {
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (command is PrivateMessage || command is Notice)
            {
                if (args[1].Any(c => c == IrcConstants.IrcColor))
                {
                    channel.IrcDaemon.Replies.SendCannotSendToChannel(user, channel.Name, "Color is not permitted in this channel");
                    return false;
                }

                if (args[1].Any(c => c == IrcConstants.IrcBold || c == IrcConstants.IrcNormal || c == IrcConstants.IrcUnderline || c == IrcConstants.IrcReverse))
                {
                    channel.IrcDaemon.Replies.SendCannotSendToChannel(user, channel.Name, "Control codes (bold/underline/reverse) are not permitted in this channel");
                    return false;
                }
            }

            return true;
        }
    }
}
