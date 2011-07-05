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
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace IrcD.Server
{
    class Engine
    {

        public static void Main(string[] args)
        {
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
            else
            {
                try
                {
                    var servicesToRun = new ServiceBase[] { new ServiceEngine() };
                    ServiceBase.Run(servicesToRun);
                }
                catch (Exception ex)
                {
                    //Log.Instance.Log(string.Format("Exception: {0} \n\nStack: {1}", ex.Message, ex.StackTrace), Level.Error);
                }
            }
        }

        public static void Start()
        {
            var settings = new Settings();
            var ircDaemon = new IrcDaemon(settings.GetIrcMode());
            settings.setDaemon(ircDaemon);
            settings.LoadSettings();

            var serverThread = new Thread(ircDaemon.Start);
            serverThread.IsBackground = false;
            serverThread.Name = "serverThread-1";
            
            serverThread.Start();
        }
    }
}