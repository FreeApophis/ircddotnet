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
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.ServerReplies;
using IrcD.Tools;
#if DEBUG
using System.Text;

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

            if (_commandList.TryGetValue(command, out CommandBase commandObject))
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
                if (args.Count < checkParamCount?.MinimumParameterCount)
                {
                    _ircDaemon.Replies.SendNeedMoreParams(info);
                    skipHandle = true;
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
