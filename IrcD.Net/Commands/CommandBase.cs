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
            this.name = name;
            this.p10 = p10;
        }

        protected IrcDaemon IrcDaemon;

        private readonly string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        private readonly string p10;

        public string P10Token
        {
            get
            {
                return p10;
            }
        }

        private long callCountIn;
        public long CallCountIn
        {
            get
            {
                return callCountIn;
            }
        }

        private long callCountOut;
        public long CallCountOut
        {
            get
            {
                return callCountOut;
            }
        }

        private long byteCountIn;
        public long ByteCountIn
        {
            get
            {
                return byteCountIn;
            }
        }
        private long byteCountOut;
        public long ByteCountOut
        {
            get
            {
                return byteCountOut;
            }
        }
        
        abstract protected void PrivateHandle(UserInfo info, List<string> args);
        public void Handle(UserInfo info, List<string> args, int bytes)
        {
            if (bytes > 0)
            {
                callCountIn++;
                byteCountIn += bytes;
            }
            PrivateHandle(info, args);
        }

        abstract protected int PrivateSend(CommandArgument argument);
        public void Send(CommandArgument argument)
        {
            callCountOut++;
            if (argument.Name == Name)
            {
                byteCountOut += PrivateSend(argument);
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
            return arg.Split(new[] { ',' });
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
