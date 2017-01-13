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

namespace IrcD.Modes
{
    public class ModeFactory
    {
        public T GetConstructor<T>() where T : Mode, new()
        {
            return new T();
        }

        public delegate ChannelMode ConstructChannelMode();
        public delegate ChannelRank ConstructChannelRank();
        public delegate UserMode ConstructUserMode();

        #region Channel Mode
        private readonly Dictionary<char, ConstructChannelMode> _channelModeFactory = new Dictionary<char, ConstructChannelMode>();

        public T AddChannelMode<T>() where T : ChannelMode, new()
        {
            var mode = GetConstructor<T>();
            _channelModeFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public ChannelMode GetChannelMode(char c)
        {
            ConstructChannelMode channelMode;
            return _channelModeFactory.TryGetValue(c, out channelMode) ? channelMode.Invoke() : null;
        }
        #endregion

        #region Channel Rank
        private readonly Dictionary<char, ConstructChannelRank> _channelRankFactory = new Dictionary<char, ConstructChannelRank>();

        public T AddChannelRank<T>() where T : ChannelRank, new()
        {
            var mode = GetConstructor<T>();
            _channelRankFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public ChannelRank GetChannelRank(char c)
        {
            ConstructChannelRank channelRank;
            return _channelRankFactory.TryGetValue(c, out channelRank) ? channelRank.Invoke() : null;
        }
        #endregion

        #region User Mode
        private readonly Dictionary<char, ConstructUserMode> _userModeFactory = new Dictionary<char, ConstructUserMode>();

        public T AddUserMode<T>() where T : UserMode, new()
        {
            var mode = GetConstructor<T>();
            _userModeFactory.Add(mode.Char, GetConstructor<T>);
            return mode;
        }

        public UserMode GetUserMode(char c)
        {
            ConstructUserMode userMode;
            return _userModeFactory.TryGetValue(c, out userMode) ? userMode.Invoke() : null;
        }
        #endregion
    }
}
