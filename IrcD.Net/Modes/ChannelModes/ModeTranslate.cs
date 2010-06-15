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
using IrcD.ServerReplies;
using IrcD.Utils;
using System.Runtime.Remoting.Messaging;
using Enumerable = System.Linq.Enumerable;

namespace IrcD.Modes.ChannelModes
{
    class ModeTranslate : ChannelMode
    {
        private readonly GoogleTranslate translator;

        public ModeTranslate()
            : base('T')
        {
            translator = new GoogleTranslate();
        }

        public override bool HandleEvent(IrcCommandType ircCommand, ChannelInfo channel, UserInfo user, List<string> args)
        {
            if (ircCommand == IrcCommandType.Join)
            {

            }
            if (ircCommand == IrcCommandType.PrivateMessage)
            {
                var t = new GoogleTranslate.TranslateMultipleDelegate(translator.TranslateText);
                t.BeginInvoke(args[1], channel.Users.Select(u => u.Languages.First()).Distinct(), TranslateCallBack, new Utils.Tuple<ChannelInfo, UserInfo>(channel, user));

                return false;
            }
            return true;
        }

        private static void TranslateCallBack(IAsyncResult asyncResult)
        {
            var state = (Utils.Tuple<ChannelInfo, UserInfo>)asyncResult.AsyncState;
            var asyncDelegate = ((AsyncResult)asyncResult).AsyncDelegate;
            var result = ((GoogleTranslate.TranslateMultipleDelegate)asyncDelegate).EndInvoke(asyncResult);

            foreach (var user in state.First.Users.Where(u => u != state.Second))
            {
                Utils.Tuple<string, string, string> res;
                if (user.Languages.Contains(result[GoogleTranslate.Original].First))
                {
                    user.IrcDaemon.Send.PrivateMessage(state.Second, user, state.First.Name, "[" + result[GoogleTranslate.Original].First + "]" + result[GoogleTranslate.Original].Third);
                }
                else if (result.TryGetValue(user.Languages.First(), out res))
                {
                    user.IrcDaemon.Send.PrivateMessage(state.Second, user, state.First.Name, "[" + res.First + "]" + res.Third);
                }
                else
                {
                    user.IrcDaemon.Send.PrivateMessage(state.Second, user, state.First.Name, "Translation Failed");
                }
            }
        }
    }
}
