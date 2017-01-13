/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2017 Thomas Bruderer <apophis@apophis.ch>
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
                UserPerChannelInfo upci;
                if (!channel.UserPerChannelInfos.TryGetValue(user.Nick, out upci))
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