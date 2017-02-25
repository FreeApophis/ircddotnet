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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Core;

namespace IrcD.Modes
{
    public class ModeList<TMode> : SortedList<char, TMode> where TMode : Mode
    {

        protected IrcDaemon IrcDaemon;

        public ModeList(IrcDaemon ircDaemon)
        {
            IrcDaemon = ircDaemon;
        }

        public void Add<T>(T element) where T : TMode
        {
            bool exists = (bool)typeof(ModeList<TMode>).GetMethod("Exist").MakeGenericMethod(new[] { element.GetType() }).Invoke(this, null);
            if (exists == false)
            {
                Add(element.Char, element);
            }
        }

        public T GetMode<T>() where T : TMode
        {
            return Values.FirstOrDefault(mode => mode is T) as T;
        }

        public void RemoveMode<T>() where T : TMode
        {
            if (Exist<T>())
            {
                var mode = GetMode<T>();
                Remove(mode.Char);
            }
        }

        public override string ToString()
        {
            var modes = new StringBuilder();

            foreach (var mode in Values)
            {
                modes.Append(mode.Char);
            }

            return modes.ToString();
        }

        public bool Exist<TExist>() where TExist : TMode
        {
            return Values.Any(m => m is TExist);
        }

    }
}
