/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2010 Thomas Bruderer <apophis@apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using IrcD.ServerReplies;

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
        /// <param name="ircCommand">Type of command</param>
        /// <param name="channel">The Channel the Mode is operating</param>
        /// <param name="user">The User which uses the Command on the channel</param>
        /// <returns>Handle Event should return true when the command is allowed to proceed normally. 
        /// It should return false, if the Mode forbids the further execution of the Command.</returns>
        public abstract bool HandleEvent(IrcCommandType ircCommand, ChannelInfo channel, UserInfo user, List<string> args);
    }
}
