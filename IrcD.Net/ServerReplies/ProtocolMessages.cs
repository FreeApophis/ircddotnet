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

using System.Text;

namespace IrcD.ServerReplies
{
    public class ProtocolMessages
    {
        private readonly IrcDaemon ircDaemon;

        public ProtocolMessages(IrcDaemon ircDaemon)
        {
            this.ircDaemon = ircDaemon;
        }

        private readonly StringBuilder command = new StringBuilder();

        private void BuildMessageHeader(UserInfo sender)
        {
            command.Length = 0;
            command.Append(sender.Prefix);
        }



        internal void Nick(UserInfo sender, InfoBase receiver, string newnick)
        {
            BuildMessageHeader(sender);

            command.Append(" NICK ");
            command.Append(newnick);

            receiver.WriteLine(command);
        }

        internal void Join(UserInfo sender, UserInfo receiver, ChannelInfo chan)
        {
            BuildMessageHeader(sender);

            command.Append(" JOIN ");
            command.Append(chan.Name);

            receiver.WriteLine(command);
        }

        internal void Part(UserInfo sender, UserInfo receiver, ChannelInfo chan, string msg)
        {
            BuildMessageHeader(sender);

            command.Append(" PART ");
            command.Append(chan.Name);
            command.Append(" :");
            command.Append(msg);

            receiver.WriteLine(command);
        }

        internal void Topic(UserInfo sender, UserInfo receiver, string chan, string newtopic)
        {
            BuildMessageHeader(sender);

            command.Append(" TOPIC ");
            command.Append(chan);
            command.Append(" :");
            command.Append(newtopic);

            receiver.WriteLine(command);
        }

        internal void Mode(UserInfo sender, UserInfo receiver, string target, string modestring)
        {
            BuildMessageHeader(sender);

            command.Append(" MODE ");
            command.Append(target);
            command.Append(" ");
            command.Append(modestring);

            receiver.WriteLine(command);
        }


        internal void PrivateMessage(UserInfo sender, UserInfo receiver, string target, string message)
        {
            BuildMessageHeader(sender);

            command.Append(" PRIVMSG ");
            command.Append(target);
            command.Append(" :");
            command.Append(message);

            receiver.WriteLine(command);
        }

        internal void Notice(UserInfo sender, UserInfo receiver, string target, string message)
        {
            BuildMessageHeader(sender);

            command.Append(" NOTICE ");
            command.Append(target);
            command.Append(" :");
            command.Append(message);

            receiver.WriteLine(command);
        }

        internal void Quit(UserInfo sender, InfoBase receiver, string message)
        {
            BuildMessageHeader(sender);

            command.Append(" QUIT :");
            command.Append(message);

            receiver.WriteLine(command);
        }

        public void Ping(InfoBase receiver)
        {
            command.Length = 0;
            command.Append(" PING ");
            command.Append(ircDaemon.ServerPrefix);

            receiver.WriteLine(command);
        }

        public void Pong(InfoBase receiver, string parameter)
        {
            command.Length = 0;
            command.Append(ircDaemon.ServerPrefix);
            command.Append(" PONG ");
            command.Append(ircDaemon.ServerPrefix);
            command.Append(" ");
            command.Append(parameter);

            receiver.WriteLine(command);
        }
    }
}
