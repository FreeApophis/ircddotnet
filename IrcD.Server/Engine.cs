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
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using IrcD.Core;
using IrcD.Core.Utils;

namespace IrcD.Server
{
    static class Engine
    {
        private static bool blocking = false;

        public static void Main(string[] args)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    Start();
                    break;
                case PlatformID.Unix:
                    Start();
                    break;
                case PlatformID.Win32NT:
                    if (Environment.UserInteractive)
                    {
                        var parameter = string.Concat(args);
                        switch (parameter)
                        {
                            case "--install":
                                ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                                return;
                            case "--uninstall":
                                ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                                return;
                        }
                        /* blocking */
                        Start();
                    }

                    try
                    {
                        var servicesToRun = new ServiceBase[] { new ServiceEngine() };
                        ServiceBase.Run(servicesToRun);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: {0} \n\nStack: {1}", ex.Message, ex.StackTrace);
                    }
                    break;
                case PlatformID.Win32S:
                    Console.WriteLine("16bit OS not supported... (STOP)");
                    break;
                case PlatformID.Win32Windows:
                    Start();
                    break;
                case PlatformID.WinCE:
                    Start();
                    break;
                case PlatformID.Xbox:
                    Start();
                    break;
                default:
                    Console.WriteLine("What kind of Platform are you? (STOP)");
                    break;
            }
        }

        public static void Start()
        {
            var settings = new Settings();
            var ircDaemon = new IrcDaemon(settings.GetIrcMode());
            settings.SetDaemon(ircDaemon);
            settings.LoadSettings();

            if (blocking)
            {
                ircDaemon.Start();
            }
            else {
                ircDaemon.ServerRehash += ServerRehash;

                var serverThread = new Thread(ircDaemon.Start)
                {
                    IsBackground = false,
                    Name = "serverThread-1"
                };

                serverThread.Start();
            }
        }

        static void ServerRehash(object sender, RehashEventArgs e)
        {
            var settings = new Settings();
            settings.SetDaemon(e.IrcDaemon);
            settings.LoadSettings();
        }
    }
}