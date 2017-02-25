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