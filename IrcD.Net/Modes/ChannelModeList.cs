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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Channel;
using IrcD.Commands;
using IrcD.Commands.Arguments;
using IrcD.Core;
using IrcD.Modes.ChannelModes;

namespace IrcD.Modes
{
    public class ChannelModeList : ModeList<ChannelMode>
    {
        public ChannelModeList(IrcDaemon ircDaemon)
            : base(ircDaemon)
        {

        }

        public bool IsSecret()
        {
            return Values.Any(mode => mode is ModeSecret);
        }

        public bool IsPrivate()
        {
            return Values.Any(mode => mode is ModePrivate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="args"></param>
        /// <returns>returns true if all Modes return true and therefore don't stop the execution of the Command</returns>
        public bool HandleEvent(CommandBase command, ChannelInfo channel, UserInfo user, List<string> args)
        {
            return Values.All(mode => mode.HandleEvent(command, channel, user, args));
        }

        internal void Update(UserInfo info, ChannelInfo chan, IEnumerable<string> args)
        {
            // In
            var sentPrivNeeded = false;
            var plus = (args.First().Length == 1) ? (bool?)null : true;
            var parameterTail = args.Skip(1);

            // Out: this is the final mode message
            var lastprefix = ' ';
            var validmode = new StringBuilder();
            var validparam = new List<string>();

            foreach (var modechar in args.First())
            {
                if (modechar == '+' || modechar == '-')
                {
                    plus = (modechar == '+');
                    continue;
                }

                var cmode = IrcDaemon.ModeFactory.GetChannelMode(modechar);
                if (cmode != null && !chan.UserPerChannelInfos[info.Nick].Modes.CurrentRank.CanChangeChannelMode(cmode))
                {
                    if (!sentPrivNeeded)
                    {
                        IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(info, chan);
                        sentPrivNeeded = true;
                    }
                    continue;
                }

                var crank = IrcDaemon.ModeFactory.GetChannelRank(modechar);
                if (crank != null && !chan.UserPerChannelInfos[info.Nick].Modes.CurrentRank.CanChangeChannelRank(crank))
                {
                    if (!sentPrivNeeded)
                    {
                        IrcDaemon.Replies.SendChannelOpPrivilegesNeeded(info, chan);
                        sentPrivNeeded = true;
                    }
                    continue;
                }

                if (plus == null)
                {
                    if (cmode is IParameterListA list)
                    {
                        if (TryGetValue(cmode.Char, out ChannelMode channelMode))
                        {
                            ((IParameterListA)channelMode).SendList(info, chan);
                        }
                        else
                        {
                            //No list yet, Send empty List
                            list.SendList(info, chan);
                        }
                        return;
                    }

                    plus = true;
                }


                if (cmode is IParameter iParam)
                {
                    var parameter = parameterTail.FirstOrDefault();
                    if (parameter != null)
                    {
                        parameterTail = parameterTail.Skip(1);
                        if (plus.Value)
                        {
                            if (!ContainsKey(cmode.Char))
                            {
                                Add(cmode);
                            }

                            if (lastprefix != '+')
                            {
                                validmode.Append(lastprefix = '+');
                            }
                            validmode.Append(cmode.Char);

                            validparam.Add(((IParameter)this[cmode.Char]).Add(parameter));

                        }
                        else
                        {
                            if (ContainsKey(cmode.Char))
                            {
                                if (this[cmode.Char] is not IParameterListA paramA)
                                {
                                    Remove(cmode.Char);
                                    if (lastprefix != '-')
                                    {
                                        validmode.Append(lastprefix = '-');
                                    }
                                    validmode.Append(cmode.Char);
                                    validparam.Add(parameter);
                                }
                                else
                                {
                                    var p = paramA.Remove(parameter);

                                    if (lastprefix != '-')
                                    {
                                        validmode.Append(lastprefix = '-');
                                    }

                                    if (p != null)
                                    {
                                        validmode.Append(cmode.Char);
                                        validparam.Add(p);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (cmode != null)
                {
                    // Channel Mode without a parameter
                    if (plus.Value)
                    {
                        if (!ContainsKey(cmode.Char))
                        {
                            Add(cmode);
                            if (lastprefix != '+')
                            {
                                validmode.Append(lastprefix = '+');
                            }
                            validmode.Append(cmode.Char);
                        }
                    }
                    else
                    {
                        if (ContainsKey(cmode.Char))
                        {
                            Remove(cmode.Char);
                            if (lastprefix != '-')
                            {
                                validmode.Append(lastprefix = '-');
                            }
                            validmode.Append(cmode.Char);
                        }
                    }
                }
                else if (crank != null)
                {
                    var parameter = parameterTail.FirstOrDefault();
                    if (parameter != null)
                    {
                        parameterTail = parameterTail.Skip(1);
                        if (chan.UserPerChannelInfos.TryGetValue(parameter, out UserPerChannelInfo upci))
                        {
                            if (plus.Value)
                            {
                                if (!upci.Modes.ContainsKey(crank.Char))
                                {
                                    upci.Modes.Add(crank);
                                    if (lastprefix != '+')
                                    {
                                        validmode.Append(lastprefix = '+');
                                    }
                                    validmode.Append(crank.Char);
                                    validparam.Add(parameter);
                                }
                            }
                            else
                            {
                                if (upci.Modes.ContainsKey(crank.Char))
                                {
                                    upci.Modes.Remove(crank.Char);
                                    if (lastprefix != '-')
                                    {
                                        validmode.Append(lastprefix = '-');
                                    }
                                    validmode.Append(crank.Char);
                                    validparam.Add(parameter);
                                }
                            }
                        }
                        else
                        {
                            info.IrcDaemon.Replies.SendUserNotInChannel(info, chan.Name, parameter);
                        }
                    }
                }
                else
                {
                    info.IrcDaemon.Replies.SendUnknownMode(info, chan, modechar);
                }
            }

            // Integrate Parameters into final mode string
            foreach (var param in validparam)
            {
                validmode.Append(" ");
                validmode.Append(param);
            }

            info.IrcDaemon.Commands.Send(new ModeArgument(info, chan, chan.Name, validmode.ToString()));
        }

        public string ToParameterList()
        {
            var modes = new StringBuilder();

            foreach (var mode in Values.Where(m => m is IParameterListA))
            {
                modes.Append(mode.Char);
            }

            modes.Append(',');
            foreach (var mode in Values.Where(m => m is IParameterB))
            {
                modes.Append(mode.Char);
            }

            modes.Append(',');
            foreach (var mode in Values.Where(m => m is IParameterC))
            {
                modes.Append(mode.Char);
            }

            modes.Append(',');
            foreach (var mode in Values.Where(m => !(m is IParameterListA) && !(m is IParameterB) && !(m is IParameterC)))
            {
                modes.Append(mode.Char);
            }

            return modes.ToString();
        }

        public string ToChannelModeString()
        {
            var modes = new StringBuilder("+");
            var parameters = new StringBuilder();

            foreach (var mode in Values.Where(m => !(m is IParameterListA)).OrderBy(c => c.Char))
            {
                modes.Append(mode.Char);
                if (mode is IParameterB b)
                {
                    parameters.Append(" ");
                    parameters.Append(b.Parameter);
                }
                if (mode is IParameterC c)
                {
                    parameters.Append(" ");
                    parameters.Append(c.Parameter);
                }
            }

            return modes.ToString() + parameters;
        }
    }
}