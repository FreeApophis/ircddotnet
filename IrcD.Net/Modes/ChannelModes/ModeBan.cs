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
using System.Linq;
using IrcD.Channel;
using IrcD.Commands;
using IrcD.Utils;

namespace IrcD.Modes.ChannelModes
{
    public class ModeBan : ChannelMode, IParameterListA
    {
        public ModeBan()
            : base('b')
        {
        }

        private readonly List<string> _banList = new List<string>();
        public List<string> Parameter => _banList;

        public void SendList(UserInfo info, ChannelInfo chan)
        {
            foreach (var ban in _banList)
            {
                info.IrcDaemon.Replies.SendBanList(info, chan, ban);
            }
            info.IrcDaemon.Replies.SendEndOfBanList(info, chan);
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (command is Join)
            {
                if (_banList.Select(ban => new WildCard(ban, WildcardMatch.Exact)).Any(usermask => usermask.IsMatch(user.Usermask)))
                {
                    user.IrcDaemon.Replies.SendBannedFromChannel(user, channel);
                    return false;
                }
            }

            if (command is PrivateMessage || command is Notice)
            {
                if (_banList.Select(ban => new WildCard(ban, WildcardMatch.Exact)).Any(usermask => usermask.IsMatch(user.Usermask)))
                {
                    user.IrcDaemon.Replies.SendCannotSendToChannel(user, channel.Name, "You are banned from the Channel");
                    return false;
                }
            }

            return true;
        }

        public string Add(string parameter)
        {
            parameter = UserInfo.NormalizeHostmask(parameter);

            _banList.Add(parameter);

            return parameter;
        }

        public string Remove(string parameter)
        {
            parameter = UserInfo.NormalizeHostmask(parameter);

            return _banList.RemoveAll(p => p == parameter) > 0 ? parameter : null;
        }
    }
}
