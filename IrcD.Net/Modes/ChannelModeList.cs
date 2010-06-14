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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Modes.ChannelModes;
using IrcD.ServerReplies;

namespace IrcD.Modes
{
    public class ChannelModeList : ModeList<ChannelMode>
    {
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
        /// <param name="ircCommand"></param>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="args"></param>
        /// <returns>returns true if all Modes return true and therefore don't stop the execution of the Command</returns>
        public bool HandleEvent(IrcCommandType ircCommand, ChannelInfo channel, UserInfo user, List<string> args)
        {
            return Values.All(mode => mode.HandleEvent(ircCommand, channel, user, args));
        }

        internal void Update(UserInfo info, ChannelInfo chan, IEnumerable<string> args)
        {
            // In
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

                var cmode = ModeFactory.GetChannelMode(modechar);
                var crank = ModeFactory.GetChannelRank(modechar);

                if (plus == null)
                {
                    var list = cmode as IParameterListA;
                    if (list != null)
                    {
                        ChannelMode channelMode;
                        if (TryGetValue(cmode.Char, out channelMode))
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

                var iParam = cmode as IParameter;

                if (iParam != null)
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
                                var paramA = this[cmode.Char] as IParameterListA;
                                if (paramA == null)
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
                        UserPerChannelInfo upci;
                        parameterTail = parameterTail.Skip(1);
                        if (chan.UserPerChannelInfos.TryGetValue(parameter, out upci))
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
            info.IrcDaemon.Send.Mode(info, chan, chan.Name, validmode.ToString());
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
                if (mode is IParameterB)
                {
                    parameters.Append(" ");
                    parameters.Append(((IParameterB)mode).Parameter);
                }
                if (mode is IParameterC)
                {
                    parameters.Append(" ");
                    parameters.Append(((IParameterC)mode).Parameter);
                }
            }

            return modes.ToString() + parameters;
        }
    }
}