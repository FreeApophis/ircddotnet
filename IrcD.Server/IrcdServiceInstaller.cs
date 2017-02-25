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

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace IrcD.Server
{
    [RunInstaller(true)]
    public class IrcdServiceInstaller : Installer
    {
        private readonly ServiceProcessInstaller _processInstaller;
        private readonly ServiceInstaller _serviceInstaller;

        public IrcdServiceInstaller()
        {
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();

            _processInstaller.Account = ServiceAccount.LocalSystem;
            _processInstaller.Username = null;
            _processInstaller.Password = null;

            //# Service Information
            _serviceInstaller.DisplayName = ServiceEngine.IrcdServiceName;
            _serviceInstaller.StartType = ServiceStartMode.Automatic;
            _serviceInstaller.ServiceName = ServiceEngine.IrcdServiceName;

            Installers.Add(_processInstaller);
            Installers.Add(_serviceInstaller);
        }
    }
}