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

namespace IrcD.Modes.ChannelModes
{
    public class ModeBanException : ChannelMode, IParameterListA
    {
        public ModeBanException()
            : base('e')
        {
        }

        private readonly List<string> banExceptionList = new List<string>();

        public List<string> Parameter
        {
            get { return banExceptionList; }
        }

        public void SendList(UserInfo info, ChannelInfo chan)
        {
            foreach (var banExcpetion in banExceptionList)
            {
                info.IrcDaemon.Replies.SendExceptionList(info, chan, banExcpetion);
            }
            info.IrcDaemon.Replies.SendEndOfExceptionList(info, chan);
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            // Handling JOIN is done in the ModeBan class
            return true;
        }

        public string Add(string parameter)
        {
            parameter = UserInfo.NormalizeHostmask(parameter);

            banExceptionList.Add(parameter);

            return parameter;
        }

        public string Remove(string parameter)
        {
            parameter = UserInfo.NormalizeHostmask(parameter);

            return banExceptionList.RemoveAll(p => p == parameter) > 0 ? parameter : null;
        }

        public override IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Repeat("EXCEPTS=" + Char, 1);
        }
    }
}

