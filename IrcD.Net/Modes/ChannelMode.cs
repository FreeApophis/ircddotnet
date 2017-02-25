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

namespace IrcD.Modes
{
    public abstract class ChannelMode : Mode
    {
        protected ChannelMode(char mode)
            : base(mode)
        {

        }

        /// <summary>
        /// This Method is called to check all the modes on a channel, each Mode has the chance to take control over a command.
        ///  If it takes control it should return false, therefore the other Commands are not checked, and the control flow will interupt.
        /// </summary>
        /// <param name="command">Type of command</param>
        /// <param name="channel">The Channel the Mode is operating</param>
        /// <param name="user">The User which uses the Command on the channel</param>
        /// <param name="args"></param>
        /// <returns>Handle Event should return true when the command is allowed to proceed normally.
        /// It should return false, if the Mode forbids the further execution of the Command.</returns>
        public abstract bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args);

        /// <summary>
        ///  Returns a list of ISUPPORT / Numeric 005 information this Mode is providing.
        /// </summary>
        /// <param name="ircDaemon">Server Object</param>
        /// <returns>returns strings for direct usage in an ISUPPORT reply</returns>
        public virtual IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Empty<string>();
        }
    }
}
