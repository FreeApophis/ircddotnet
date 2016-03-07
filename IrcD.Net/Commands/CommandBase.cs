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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Commands.Arguments;

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
            var argument = commandArgument as T;

            if (argument == null)
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
