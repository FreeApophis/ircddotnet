/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 19.03.2009
 * Zeit: 15:18
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace IrcD
{
    class Program
    {
        public static void Main(string[] args)
        {
            IrcDaemon ircd = new IrcDaemon();
            
            ircd.ServerPass = null;
            ircd.ServerName = "apophis.ch";
            
            ircd.MainLoop();
        }
    }

}