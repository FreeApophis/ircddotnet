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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Commands.Arguments;
using IrcD.Core;

namespace IrcD.Commands
{
    public abstract class CommandBase
    {
        protected CommandBase(IrcDaemon ircDaemon, string name, string p10)
        {
            IrcDaemon = ircDaemon;
            Name = name;
            P10Token = p10;
        }

        protected IrcDaemon IrcDaemon;

        public string Name { get; }
        public string P10Token { get; }

        public long CallCountIn { get; private set; }
        public long CallCountOut { get; private set; }

        public long ByteCountIn { get; private set; }
        public long ByteCountOut { get; private set; }

        protected abstract void PrivateHandle(UserInfo info, List<string> args);
        public void Handle(UserInfo info, List<string> args, int bytes)
        {
            if (bytes > 0)
            {
                CallCountIn++;
                ByteCountIn += bytes;
            }
            PrivateHandle(info, args);
        }

        protected abstract int PrivateSend(CommandArgument argument);
        public void Send(CommandArgument argument)
        {
            CallCountOut++;
            if (argument.Name == Name)
            {
                ByteCountOut += PrivateSend(argument);
            }
            else
            {
                // if the wrong Send is called, we requeue argument in the Servers CommandList
                IrcDaemon.Commands.Send(argument);
            }
        }

        protected readonly StringBuilder Command = new StringBuilder();

        protected void BuildMessageHeader(CommandArgument commandArgument)
        {
            Command.Length = 0;
            Command.Append(commandArgument.Sender == null ? IrcDaemon.ServerPrefix : commandArgument.Sender.Prefix);
            Command.Append(" ");
            Command.Append(commandArgument.Name);
            Command.Append(" ");
        }

        protected T GetSaveArgument<T>(CommandArgument commandArgument) where T : CommandArgument
        {
            if (commandArgument is not T argument)
            {
                throw new InvalidCastException("this shall not happen");
            }

            return argument;
        }

        public static string[] GetSubArgument(string arg)
        {
            return arg.Split(',');
        }

        /// <summary>
        ///  Returns a list of ISUPPORT / Numeric 005 information this Mode is providing.
        /// </summary>
        /// <param name="ircDaemon">Server Object</param>
        /// <returns>returns strings for direct usage in an ISUPPORT reply</returns>
        public virtual IEnumerable<string> Support(IrcDaemon ircDaemon)
        {
            return Enumerable.Empty<string>();
        }
    }
}
