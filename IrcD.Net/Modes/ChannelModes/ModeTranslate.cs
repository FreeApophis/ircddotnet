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


using System.Collections.Generic;
using IrcD.ServerReplies;
using IrcD.Utils;

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
                // TODO: dangerously slow!!! not allowed!!! BLOCKS!!!
                var source = translator.DetectLanguage(args[1]);
                var text = translator.TranslateText(args[1], source, "en");
                channel.IrcDaemon.Send.PrivateMessage(user, channel, channel.Name, "[" + source + "]" + text);
                return false;
            }
            return true;
        }
    }
}
