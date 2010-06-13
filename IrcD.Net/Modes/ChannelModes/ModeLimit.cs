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

using System;
using System.Collections.Generic;
using IrcD.ServerReplies;

namespace IrcD.Modes.ChannelModes
{
    public class ModeLimit : ChannelMode, IParameterC
    {
        public ModeLimit() :
            base('l')
        {

        }

        private int limit;

        public string Parameter
        {
            get
            {
                return limit.ToString();
            }
            set
            {
                SetLimit(value);
            }
        }

        private void SetLimit(string value)
        {
            int.TryParse(value, out limit);
            if (limit < 1)
            {
                limit = 1;
            }
        }

        public override bool HandleEvent(IrcCommandType ircCommand, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (ircCommand == IrcCommandType.Join)
            {

                if (limit <= channel.UserPerChannelInfos.Count)
                {
                    user.IrcDaemon.Replies.SendChannelIsFull(user, channel);
                    return false;
                }
            }
            return true;
        }

        public string Add(string parameter)
        {
            SetLimit(parameter);
            return Parameter;
        }
    }
}
