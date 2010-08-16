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
using System.Reflection;
using System.Threading;

#if !UBUNTU
using System.Linq;
using IrcD.Database;
using System.Configuration.Install;
using System.ServiceProcess;
#endif

namespace IrcD
{
    class Engine
    {

        public static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
#if !UBUNTU
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
#endif
                /* blocking */
                Start();
            }
#if !UBUNTU
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
#endif
        }

        public static void Start()
        {
            var ircd1 = new IrcDaemon();

            ircd1.Options.MaxNickLength = 50;
            ircd1.Options.ServerPorts.Clear();
            ircd1.Options.ServerPorts.Add(6667);
            ircd1.Options.ServerPorts.Add(6668);

#if !UBUNTU
            ircd1.Options.ServerPass = (from setting in DatabaseCommon.Db.Settings where setting.Key == "ServerPass" select setting.Value).SingleOrDefault();
            ircd1.Options.ServerName = (from setting in DatabaseCommon.Db.Settings where setting.Key == "ServerName" select setting.Value).SingleOrDefault();
            ircd1.Options.MessageOfTheDay = (from setting in DatabaseCommon.Db.Settings where setting.Key == "MessageOfTheDay" select setting.Value).SingleOrDefault();
#endif
            ircd1.Options.ServerName = "test.ch";
            var t = new Thread(ircd1.Start);
            t.Start();

            var ircd2 = new IrcDaemon();

            ircd2.Options.MaxNickLength = 50;
            ircd2.Options.ServerPorts.Clear();
            ircd2.Options.ServerPorts.Add(6669);

            ircd2.Start();
        }
    }
}