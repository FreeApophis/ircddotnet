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
using IrcD.Modes.ChannelModes;
using IrcD.Modes.ChannelRanks;
using IrcD.Modes.UserModes;

namespace IrcD.Modes
{
    class ModeFactory
    {
        public delegate T Construct<out T>();

        private static readonly Dictionary<char, Construct<ChannelMode>> ChannelFactory = new Dictionary<char, Construct<ChannelMode>>();

        public static T GetConstructor<T>() where T : Mode, new()
        {
            return new T();
        }

        public static ChannelMode GetChannelMode(char c)
        {
            Construct<ChannelMode> channelMode;
            if (ChannelFactory.TryGetValue(c, out channelMode))
            {
                return channelMode.Invoke();
            }

            switch (c)
            {
                case 'b': return new ModeBan();
                case 'c': return new ModeColorless();
                case 'e': return new ModeBanException();
                case 'k': return new ModeKey();
                case 'l': return new ModeLimit();
                case 'm': return new ModeModerated();
                case 'n': return new ModeNoExternal();
                case 'p': return new ModePrivate();
                case 's': return new ModeSecret();
                case 't': return new ModeTopic();
                default: return null;
            }
        }

        public static ChannelRank GetChannelRank(char c)
        {
            switch (c)
            {
                case 'v': return new ModeVoice();
                case 'h': return new ModeHalfOp();
                case 'o': return new ModeOp();
                default: return null;
            }
        }

        public static UserMode GetUserMode(char c)
        {
            switch (c)
            {
                case 'i': return new ModeInvisible();
                case 'r': return new ModeRestricted();
                case 'w': return new ModeWallops();
                default: return null;
            }
        }
    }
}
