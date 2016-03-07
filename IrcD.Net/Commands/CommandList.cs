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
using IrcD.Commands.Arguments;
using IrcD.ServerReplies;

#if DEBUG
using System.Text;
using IrcD.Utils;
#endif

namespace IrcD.Commands
{

    public class CommandList : IEnumerable<CommandBase>
    {
        private readonly Dictionary<string, CommandBase> _commandList;
        private readonly IrcDaemon _ircDaemon;

        public CommandList(IrcDaemon ircDaemon)
        {
            _commandList = new Dictionary<string, CommandBase>(StringComparer.OrdinalIgnoreCase);
            _ircDaemon = ircDaemon;
        }
        public void Add(CommandBase command)
        {
            _commandList.Add(command.Name, command);
        }

        public IEnumerable<string> Supported()
        {
            return _commandList.SelectMany(m => m.Value.Support(_ircDaemon));
        }


        public void Handle(UserInfo info, string prefix, ReplyCode replyCode, List<string> args, int bytes)
        {
            throw new NotImplementedException();
        }

        public void Handle(UserInfo info, string prefix, string command, List<string> args, int bytes)
        {
            CommandBase commandObject;

            if (_commandList.TryGetValue(command, out commandObject))
            {
                bool skipHandle = false;
                var handleMethodInfo = commandObject.GetType().GetMethod("Handle");

                var checkRegistered = Attribute.GetCustomAttribute(handleMethodInfo, typeof(CheckRegisteredAttribute)) as CheckRegisteredAttribute;
                if (checkRegistered != null)
                {
                    if (!info.Registered)
                    {
                        _ircDaemon.Replies.SendNotRegistered(info);
                        skipHandle = true;
                    }
                }

                var checkParamCount = Attribute.GetCustomAttribute(handleMethodInfo, typeof(CheckParamCountAttribute)) as CheckParamCountAttribute;
                if (checkParamCount != null)
                {
                    if (args.Count < checkParamCount.MinimumParameterCount)
                    {
                        _ircDaemon.Replies.SendNeedMoreParams(info);
                        skipHandle = true;
                    }
                }


                if (!skipHandle)
                {
                    commandObject.Handle(info, args, bytes);
                    info.LastAlive = DateTime.Now;
                }
            }
            else
            {
#if DEBUG
                Logger.Log("Command " + command + "is not yet implemented");
#endif

                if (info.Registered)
                {
                    // we only inform the client about invalid commands if he is already successfully registered
                    // we dont want to make "wrong protocol ping-pong"
                    _ircDaemon.Replies.SendUnknownCommand(info, command);
                }
            }

#if DEBUG
            var parsedLine = new StringBuilder();
            parsedLine.Append("[" + info.Usermask + "]-[" + command + "]");

            foreach (var arg in args)
            {
                parsedLine.Append("-<" + arg + ">");
            }

            Logger.Log(parsedLine.ToString());
#endif
        }

        public void Send(CommandArgument argument)
        {
            CommandBase commandObject = _commandList[argument.Name];
            commandObject.Send(argument);
        }

        public IEnumerator<CommandBase> GetEnumerator()
        {
            return _commandList.Select(command => command.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _commandList.Select(command => command.Value).GetEnumerator();
        }
    }
}
