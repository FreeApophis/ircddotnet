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
using System.Text;
using IrcD.Utils;

namespace IrcD.Commands
{

    public class CommandList
    {
        private readonly Dictionary<string, CommandBase> commandList = new Dictionary<string, CommandBase>(StringComparer.OrdinalIgnoreCase);

        public void Add(CommandBase command)
        {
            commandList.Add(command.Name, command);
        }

        public void Handle(string command, UserInfo info, List<string> args)
        {
            CommandBase commandObject;

            if (commandList.TryGetValue(command, out commandObject))
            {
                commandObject.Handle(info, args);
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
                    //TODO: 
                    //SendUnknownCommand(info, command);
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
    }
}
