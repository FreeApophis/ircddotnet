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
using IrcD.Tools;

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
