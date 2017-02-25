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
using IrcD.Commands;
using IrcD.Commands.Arguments;
using IrcD.Core;

namespace IrcD.Modes
{
    public class UserModeList : ModeList<UserMode>
    {
        public UserModeList(IrcDaemon ircDaemon)
            : base(ircDaemon)
        {

        }

        public bool HandleEvent(CommandBase command, UserInfo user, List<string> args)
        {
            return Values.All(mode => mode.HandleEvent(command, user, args));
        }

        internal void Update(UserInfo info, IEnumerable<string> args)
        {
            var plus = true;
            var lastprefix = ' ';
            var validmode = new StringBuilder();

            foreach (var modechar in args.First())
            {
                if (modechar == '+' || modechar == '-')
                {
                    plus = (modechar == '+');
                    continue;
                }

                var umode = IrcDaemon.ModeFactory.GetUserMode(modechar);
                if (umode == null) continue;

                if (plus)
                {
                    if (!ContainsKey(umode.Char))
                    {
                        Add(umode);

                        if (lastprefix != '+')
                        {
                            validmode.Append(lastprefix = '+');
                        }
                        validmode.Append(umode.Char);
                    }
                }
                else
                {
                    if (ContainsKey(umode.Char))
                    {
                        Remove(umode.Char);

                        if (lastprefix != '-')
                        {
                            validmode.Append(lastprefix = '-');
                        }
                        validmode.Append(umode.Char);
                    }
                }
            }

            info.IrcDaemon.Commands.Send(new ModeArgument(info, info, info.Nick, validmode.ToString()));
        }

        public string ToUserModeString()
        {
            return "+" + ToString();
        }
    }
}