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
    public class ModeKey : ChannelMode, IParameterB
    {
        public ModeKey()
            : base('k')
        {
        }

        private string _key;

        public string Parameter
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public override bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (command is Join)
            {
                var keys = args.Count > 1 ? (IEnumerable<string>)CommandBase.GetSubArgument(args[1]) : new List<string>();

                if (keys.All(k => k != _key))
                {
                    user.IrcDaemon.Replies.SendBadChannelKey(user, channel);
                    return false;
                }
            }

            return true;
        }

        public string Add(string parameter)
        {
            _key = parameter;
            return _key;
        }
    }
}

