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

using IrcD.Modes.ChannelModes;
using IrcD.Modes.ChannelRanks;
using IrcD.Modes.UserModes;

namespace IrcD.Modes
{
    class ModeFactory
    {
        public static ChannelMode GetChannelMode(char c)
        {
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
                default: return null;
            }
        }
    }
}
