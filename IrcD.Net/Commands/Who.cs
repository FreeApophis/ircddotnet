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
using IrcD.Channel;
using IrcD.Commands.Arguments;
using IrcD.Modes.UserModes;
using IrcD.Utils;

namespace IrcD.Commands
{
    public class Who : CommandBase
    {
        public Who(IrcDaemon ircDaemon)
            : base(ircDaemon, "WHO")
        { }

        [CheckRegistered]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            IEnumerable<UserPerChannelInfo> whoList;
            var filterInvisible = true;

            if (!info.PassAccepted)
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
                return;
            }

            ChannelInfo channel;
            var mask = string.Empty;

            if (args.Count < 1 || args[0] == "0")
            {
                whoList = IrcDaemon.Nicks.SelectMany(n => n.Value.UserPerChannelInfos);
            }
            else if (args.Count > 0 && IrcDaemon.Channels.TryGetValue(args[0], out channel))
            {
                whoList = channel.UserPerChannelInfos.Values;
                if (!channel.UserPerChannelInfos.ContainsKey(info.Nick))
                {
                    filterInvisible = false;
                }
            }
            else
            {
                mask = args[0];
                var wildCard = new WildCard(mask, WildcardMatch.Anywhere);
                whoList = IrcDaemon.Nicks.Values.Where(u => wildCard.IsMatch(u.Usermask)).SelectMany(n => n.UserPerChannelInfos);
            }

            if (filterInvisible)
            {
                whoList = whoList.Where(w => !w.UserInfo.Modes.Exist<ModeInvisible>());
            }

            if (args.Count > 1 && args[1] == "o")
            {
                whoList = whoList.Where(w => w.UserInfo.Modes.Exist<ModeOperator>() || w.UserInfo.Modes.Exist<ModeLocalOperator>());
            }

            foreach (var who in whoList)
            {
                IrcDaemon.Replies.SendWhoReply(info, who);
            }
            IrcDaemon.Replies.SendEndOfWho(info, mask);
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }
    }
}
