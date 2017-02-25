/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 *  
 * Copyright (c) 2009-2017, Thomas Bruderer, apophis@apophis.ch All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *   
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * * Neither the name of ArithmeticParser nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
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
