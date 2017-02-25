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
using System.Collections.Generic;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.Modes.UserModes;

namespace IrcD.Commands
{
    public class Oper : CommandBase
    {
        public Oper(IrcDaemon ircDaemon)
            : base(ircDaemon, "OPER", "OPER")
        { }

        [CheckRegistered]
        [CheckParamCount(2)]
        protected override void PrivateHandle(UserInfo info, List<string> args)
        {
            if (DenyOper(info))
            {
                IrcDaemon.Replies.SendNoOperHost(info);
                return;
            }

            if (ValidOperLine(args[0], args[1]))
            {
                info.Modes.Add(new ModeLocalOperator());
                info.Modes.Add(new ModeOperator());

                IrcDaemon.Replies.SendYouAreOper(info);
            }
            else
            {
                IrcDaemon.Replies.SendPasswordMismatch(info);
            }
        }

        protected override int PrivateSend(CommandArgument commandArgument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if an IRC Operatur status can be granted upon user and pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool ValidOperLine(string user, string pass)
        {
            string realpass;
            if (IrcDaemon.Options.OLine.TryGetValue(user, out realpass))
            {
                if (pass == realpass)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the Host of the user is allowed to use the OPER command
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool DenyOper(UserInfo info)
        {
            var allow = false;
            foreach (var operHost in IrcDaemon.Options.OperHosts)
            {
                if (!allow && operHost.Allow)
                    allow = operHost.WildcardHostMask.IsMatch(info.Host);

                if (allow && !operHost.Allow)
                    allow = !operHost.WildcardHostMask.IsMatch(info.Host);
            }

            return !allow;
        }
    }
}

