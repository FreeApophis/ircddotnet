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
using System.Linq;
#if !UBUNTU
using IrcD.Database;
#endif
using IrcD.Modes;
using IrcD.Modes.ChannelModes;

namespace IrcD
{
    class Program
    {
        public static void Main(string[] args)
        {
            var ircd = new IrcDaemon(IrcMode.Modern);
            ircd.Options.NickLength = 50;

#if !UBUNTU
            ircd.Options.ServerPass = (from setting in DatabaseCommon.Db.Settings where setting.Key == "ServerPass" select setting.Value).SingleOrDefault();
            ircd.Options.ServerName = (from setting in DatabaseCommon.Db.Settings where setting.Key == "ServerName" select setting.Value).SingleOrDefault();
            ircd.Options.MOTD.AddRange(DatabaseCommon.Db.Settings.Where(setting => setting.Key == "MessageOfTheDay").Select(setting => setting.Value));
#endif
            ircd.Start();
        }
    }
}