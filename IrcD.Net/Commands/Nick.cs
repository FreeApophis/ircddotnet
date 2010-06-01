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

namespace IrcD.Commands
{
    public class Nick : CommandBase
    {
        public Nick(IrcDaemon ircDaemon)
            : base(ircDaemon, "NICK")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
            if (!info.PassAccepted)
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
                return;
            }
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNoNicknameGiven(info);
                return;
            }
            if (IrcDaemon.Nicks.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNicknameInUse(info, args[0]);
                return;
            }
            if (!UserInfo.ValidNick(args[0]))
            {
                IrcDaemon.Replies.SendErroneousNickname(info, args[0]);
                return;
            }

            // NICK command valid after this point

            if (!info.NickExists)
            {
                //First Nick Command
                IrcDaemon.Nicks.Add(args[0], info);
                info.InitNick(args[0]);
                return;
            }

            // Announce nick change to itself
            IrcDaemon.Send.Nick(info, info, args[0]);
            
            // Announce nick change to all channels it is in
            foreach (var channelInfo in info.Channels)
            {
                IrcDaemon.Send.Nick(info, channelInfo, args[0]);
            }
            
            info.Rename(args[0]);
        }
    }
}
