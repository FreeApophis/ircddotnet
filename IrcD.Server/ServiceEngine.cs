/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2017 Thomas Bruderer <apophis@apophis.ch>
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
using System.ServiceProcess;
using System.Threading;

namespace IrcD.Server
{
    public sealed class ServiceEngine : ServiceBase
    {
        public static string IrcdServiceName = "IRCd.net";
        private Thread _botThread;

        public ServiceEngine()
        {
            ServiceName = IrcdServiceName;

            CanHandlePowerEvent = false;
            CanPauseAndContinue = false;
            CanHandleSessionChangeEvent = false;
            CanShutdown = true;
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {

            _botThread = new Thread(Engine.Start) { IsBackground = true };
            _botThread.Start();

            base.OnStart(args);
        }


        protected override void OnStop()
        {
            _botThread.Abort();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            _botThread.Abort();
            base.OnShutdown();
        }

        public static void WriteToLog(string message, EventLogEntryType eventLogEntryType = EventLogEntryType.Information)
        {
            if (!EventLog.SourceExists(IrcdServiceName))
            {
                EventLog.CreateEventSource(IrcdServiceName, "Application");
            }

            var eventLog = new EventLog { Source = IrcdServiceName };
            eventLog.WriteEntry(message, eventLogEntryType);
        }
    }
}