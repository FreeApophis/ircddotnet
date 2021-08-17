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
using IrcD.Channel;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.Modes.UserModes;
using IrcD.Tools;

namespace IrcD.Commands
{
    public class Who : CommandBase
    {
        public Who(IrcDaemon ircDaemon)
            : base(ircDaemon, "WHO", "H")
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

            var mask = string.Empty;

            if (args.Count < 1 || args[0] == "0")
            {
                whoList = IrcDaemon.Nicks.SelectMany(n => n.Value.UserPerChannelInfos);
            }
            else if (args.Count > 0 && IrcDaemon.Channels.TryGetValue(args[0], out ChannelInfo channel))
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
