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

namespace IrcD.Modes.ChannelModes
{
    public class ModeInviteList : ChannelMode, IParameterListA
    {
        public ModeInviteList()
            : base('I')
        {
        }

        private readonly List<string> inviteList = new List<string>();

        public List<string> Parameter
        {
            get { return inviteList; }
        }

        public void SendList(UserInfo info, ChannelInfo chan)
        {
            foreach (var invite in inviteList)
            {
                info.IrcDaemon.Replies.SendInviteList(info, chan, invite);
            }
            info.IrcDaemon.Replies.SendEndOfInviteList(info, chan);
        }

        public override bool HandleEvent(IrcCommandType ircCommand, ChannelInfo channel, UserInfo user, List<string> args)
        {
            // Handling JOIN is done in the ModeInvite class
            return true;
        }

        public string Add(string parameter)
        {
            inviteList.Add(parameter);
            return parameter;
        }
    }
}
