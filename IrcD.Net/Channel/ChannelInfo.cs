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
using System.Text;
using IrcD.Core;
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
                {
                    return '*';
                }
                if (Modes.IsSecret())
                {
                    return '@';
                }

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
