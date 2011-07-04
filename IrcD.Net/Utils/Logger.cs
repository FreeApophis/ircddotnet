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

using System.Diagnostics;
using System;
using IrcD.Database;

namespace IrcD.Utils
{
    class Logger
    {
        public static void Log(string message, int level = 4, string location = null)
        {
            var stackTrace = new StackTrace();
            var callerFrame = stackTrace.GetFrame(1);

            Console.WriteLine(string.Format("{0} in {2}: {1}", level, message, location ?? callerFrame.ToString()));

            //var entity = new Log { Level = level, Message = message, Location = location ?? callerFrame.ToString(), Time = DateTime.Now };
            //DatabaseCommon.Db.Logs.InsertOnSubmit(entity);
            //DatabaseCommon.Db.SubmitChanges();
        }
    }
}
