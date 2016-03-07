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
using System.Text;
using IrcD.Modes;

namespace IrcD.Channel
{
    public class ChannelInfo : InfoBase
    {
        public ChannelInfo(string name, IrcDaemon ircDaemon)
            : base(ircDaemon)
        {
            Name = name;
            ChannelType = ircDaemon.SupportedChannelTypes[name[0]];
            Modes = new ChannelModeList(ircDaemon);
        }

        public ChannelType ChannelType { get; }

        public string Name { get; }
        public string Topic { get; set; }

        public Dictionary<string, UserPerChannelInfo> UserPerChannelInfos { get; } = new Dictionary<string, UserPerChannelInfo>();
        public IEnumerable<UserInfo> Users => UserPerChannelInfos.Select(upci => upci.Value.UserInfo);
        public ChannelModeList Modes { get; }

        public string ModeString => Modes.ToChannelModeString();

        public char NamesPrefix
        {
            get
            {
                if (Modes.IsPrivate())
                    return '*';
                if (Modes.IsSecret())
                    return '@';
                return '=';
            }
        }

        public void RemoveUser(UserInfo user)
        {
            var upci = UserPerChannelInfos[user.Nick];

            UserPerChannelInfos.Remove(user.Nick);
            user.UserPerChannelInfos.Remove(upci);

            if (UserPerChannelInfos.Any() == false)
            {
                IrcDaemon.Channels.Remove(Name);
            }
        }

        public override int WriteLine(StringBuilder line)
        {
            return Users.Sum(user => user.WriteLine(line));
        }

        public override int WriteLine(StringBuilder line, UserInfo exception)
        {
            return Users.Sum(user => user.WriteLine(line, exception));
        }


    }
}
