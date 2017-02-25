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
using System.Linq;
using IrcD.Channel;
using IrcD.Commands;
using IrcD.Core;

namespace IrcD.Modes.ChannelModes
{
    public class ModeBanException : ChannelMode, IParameterListA
    {
        public ModeBanException()
            : base('e')
        {
        }

        private readonly List<string> _banExceptionList = new List<string>();

        public List<string> Parameter => _banExceptionList;

        public void SendList(UserInfo info, ChannelInfo chan)
        {
            foreach (var banExcpetion in _banExceptionList)
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

            _banExceptionList.Add(parameter);

            return parameter;
        }

        public string Remove(string parameter)
        {
            parameter = UserInfo.NormalizeHostmask(parameter);

            return _banExceptionList.RemoveAll(p => p == parameter) > 0 ? parameter : null;
        }

        public override IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Repeat("EXCEPTS=" + Char, 1);
        }
    }
}

