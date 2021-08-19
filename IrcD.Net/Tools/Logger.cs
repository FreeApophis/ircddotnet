﻿/*
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

using Pastel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace IrcD.Tools
{
    public static class Logger
    {
        public static void Log(string message, int level = 4, string location = null)
        {
            var stackTrace = new StackTrace();
            var callerFrame = stackTrace.GetFrame(1);

            Console.WriteLine("{0} in {2}: {1}", level, message.Pastel("ffffcc"), location ?? FormatLocation(callerFrame));
        }

        public static string FormatLocation(StackFrame frame)
        {
            StringBuilder location = new StringBuilder();

            location.Append(frame.GetMethod().DeclaringType);
            location.Append("=>");
            location.Append(frame.GetMethod());
            location.Append(" [");
            location.Append(frame.GetILOffset());
            location.Append(":");
            location.Append(frame.GetNativeOffset());
            location.Append("]");

            return location.ToString();
        }
    }
}
