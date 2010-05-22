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

namespace IrcD
{
    public class ChannelInfo : InfoBase
    {
        public ChannelInfo(string name, IrcDaemon ircDaemon)
            : base(ircDaemon)
        {
            this.name = name;
        }

        private readonly string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Topic { get; set; }

        private readonly Dictionary<string, UserPerChannelInfo> userPerChannelInfos = new Dictionary<string, UserPerChannelInfo>();

        public Dictionary<string, UserPerChannelInfo> UserPerChannelInfos
        {
            get
            {
                return userPerChannelInfos;
            }
        }

        public IEnumerable<UserInfo> Users
        {
            get
            {
                return userPerChannelInfos.Select(upci => upci.Value.UserInfo);
            }
        }

        private readonly ChannelModeList modes = new ChannelModeList();

        public ChannelModeList Modes
        {
            get
            {
                return modes;
            }
        }

        public override void WriteLine(StringBuilder line)
        {
            foreach (var user in Users)
            {
                user.WriteLine(line);
            }
        }
    }
}
