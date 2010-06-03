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

namespace IrcD.Modes
{
    class ModeFactory
    {
        public static T GetConstructor<T>() where T : Mode, new()
        {
            return new T();
        }

        public delegate ChannelMode ConstructChannelMode();
        public delegate ChannelRank ConstructChannelRank();
        public delegate UserMode ConstructUserMode();

        #region Channel Mode
        private static readonly Dictionary<char, ConstructChannelMode> ChannelModeFactory = new Dictionary<char, ConstructChannelMode>();

        public static T AddChannelMode<T>() where T : ChannelMode, new()
        {
            var mode = new T();
            ChannelModeFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public static ChannelMode GetChannelMode(char c)
        {
            ConstructChannelMode channelMode;
            return ChannelModeFactory.TryGetValue(c, out channelMode) ? channelMode.Invoke() : null;
        }
        #endregion

        #region Channel Rank
        private static readonly Dictionary<char, ConstructChannelRank> ChannelRankFactory = new Dictionary<char, ConstructChannelRank>();

        public static T AddChannelRank<T>() where T : ChannelRank, new()
        {
            var mode = new T();
            ChannelRankFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public static ChannelRank GetChannelRank(char c)
        {
            ConstructChannelRank channelRank;
            return ChannelRankFactory.TryGetValue(c, out channelRank) ? channelRank.Invoke() : null;
        }
        #endregion

        #region User Mode
        private static readonly Dictionary<char, ConstructUserMode> UserModeFactory = new Dictionary<char, ConstructUserMode>();

        public static T AddUserMode<T>() where T : UserMode, new()
        {
            var mode = new T();
            UserModeFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public static UserMode GetUserMode(char c)
        {
            ConstructUserMode userMode;
            return UserModeFactory.TryGetValue(c, out userMode) ? userMode.Invoke() : null;
        }
        #endregion
    }
}
