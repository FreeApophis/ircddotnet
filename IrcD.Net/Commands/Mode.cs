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

namespace IrcD.Commands
{
    public class Mode : CommandBase
    {
        public Mode(IrcDaemon ircDaemon)
            : base(ircDaemon, "MODE")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
        }
    }
}

//internal void ModeDelegate(UserInfo info, List<string> args)
//{
//    //TODO: new modes
//    if (!info.Registered)
//    {
//        SendNotRegistered(info);
//        return;
//    }
//    if (args.Count == 0)
//    {
//        SendNeedMoreParams(info);
//        return;
//    }

//    if (ValidChannel(args[0]))
//    {
//        if (!channels.ContainsKey(args[0]))
//        {
//            // TODO: Send Chan does not exist;
//            return;
//        }
//        ChannelInfo chan = channels[args[0]];
//        if (args.Count == 1)
//        {
//            // TODO: which modes should we send?
//        }
//        else
//        {
//            string reply = "";
//            Dictionary<string, List<string>> compatibilityCache = null;
//            foreach (ModeElement cmode in ParseChannelModes(args))
//            {
//                switch (cmode.Mode)
//                {
//                    case 'a':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_a = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'b':
//                        if (cmode.Plus != null)
//                        {
//                            if (cmode.Plus.Value)
//                            {
//                                chan.Mode_b.Add(cmode.Param);
//                            }
//                            else
//                            {
//                                chan.Mode_b.Remove(cmode.Param);
//                            }
//                        }
//                        break;
//                    case 'e':
//                        if (cmode.Plus != null)
//                        {
//                            if (cmode.Plus.Value)
//                            {
//                                chan.Mode_e.Add(cmode.Param);
//                            }
//                            else
//                            {
//                                chan.Mode_e.Remove(cmode.Param);
//                            }
//                        }
//                        break;
//                    case 'h':
//                        if (cmode.Param == null || cmode.Plus == null)
//                            throw new ArgumentNullException();
//                        if (chan.User.ContainsKey(cmode.Param))
//                        {
//                            chan.User[cmode.Param].Mode_h = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'i':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_i = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'I':
//                        if (cmode.Plus != null)
//                        {
//                            if (cmode.Plus.Value)
//                            {
//                                chan.Mode_I.Add(cmode.Param);
//                            }
//                            else
//                            {
//                                chan.Mode_I.Remove(cmode.Param);
//                            }
//                        }
//                        break;
//                    case 'k':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_k = cmode.Plus.Value ? cmode.Param : null;
//                        }
//                        break;
//                    case 'l':
//                        if (cmode.Param == null)
//                            throw new ArgumentNullException();
//                        if (cmode.Plus != null)
//                        {
//                            if (cmode.Plus.Value)
//                            {
//                                chan.Mode_l = int.Parse(cmode.Param);
//                            }
//                            else
//                            {
//                                chan.Mode_l = -1;
//                            }
//                        }
//                        break;
//                    case 'm':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_m = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'n':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_n = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'o':
//                        if (cmode.Param == null || cmode.Plus == null)
//                            throw new ArgumentNullException();
//                        if (chan.User.ContainsKey(cmode.Param))
//                        {
//                            chan.User[cmode.Param].Mode_o = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'O':
//                        break;
//                    case 'p':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_p = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'q':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_q = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'r':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_r = cmode.Plus.Value;
//                        }
//                        break;
//                    case 's':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_s = cmode.Plus.Value;
//                        }
//                        break;
//                    case 't':
//                        if (cmode.Plus != null)
//                        {
//                            chan.Mode_t = cmode.Plus.Value;
//                        }
//                        break;
//                    case 'v':
//                        if (cmode.Param == null || cmode.Plus == null)
//                            throw new ArgumentNullException();
//                        if (chan.User.ContainsKey(cmode.Param))
//                        {
//                            chan.User[cmode.Param].Mode_v = cmode.Plus.Value;
//                        }
//                        break;
//                    default:
//                        SendUnknownMode(info, chan, cmode.Mode);
//                        continue;
//                }

//                if (Options.ClientCompatibilityMode)
//                {
//                    compatibilityCache = compatibilityCache ?? new Dictionary<string, List<string>>();
//                    string key = ((cmode.Plus.HasValue) ? cmode.Plus.Value ? "+" : "-" : "") + cmode.Mode;
//                    if (compatibilityCache.ContainsKey(key))
//                    {
//                        compatibilityCache[key].Add(cmode.Param ?? "");
//                    }
//                    else
//                    {
//                        compatibilityCache.Add(key, new List<string> { cmode.Param ?? "" });
//                    }
//                }
//                else
//                {
//                    //TODO: this is very convinient, but mIRC and xchat cannot parse this (RTF RFC)
//                    if (cmode.Plus == null)
//                    {
//                        reply += cmode.Mode + " ";
//                    }
//                    else if (cmode.Plus.Value)
//                    {
//                        reply += "+" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
//                    }
//                    else
//                    {
//                        reply += "-" + cmode.Mode + " " + ((cmode.Param == null) ? "" : cmode.Param + " ");
//                    }
//                }
//            }
//            if (Options.ClientCompatibilityMode && compatibilityCache != null)
//            {
//                foreach (var modes in compatibilityCache)
//                {

//                    if (modes.Key[0] == '+' || modes.Key[0] == '-')
//                    {
//                        reply += modes.Key[0];
//                        reply += new string(modes.Key[1], modes.Value.Count);
//                        foreach (var param in modes.Value)
//                        {
//                            if (param.Length > 0)
//                            {
//                                reply += " " + param;
//                            }
//                        }
//                        reply += " ";
//                    }
//                    else
//                    {
//                        reply += new string(modes.Key[0], modes.Value.Count) + " ";
//                    }
//                }
//            }

//            if (reply.Length > 0)
//            {
//                foreach (UserPerChannelInfo upci in chan.User.Values)
//                {
//                    SendMode(info, upci.Info, chan.Name, reply);
//                }
//            }

//        }
//    }
//    else if (args[0] == info.Nick)
//    {
//        if (args.Count == 1)
//        {
//            SendUserModeIs(info);
//        }
//        else
//        {
//            string reply = "";
//            foreach (KeyValuePair<char, bool> umode in ParseUserMode(args[1]))
//            {
//                switch (umode.Key)
//                {
//                    case 'i':
//                        info.Mode_i = umode.Value;
//                        break;
//                    case 'O':
//                        if (!umode.Value)
//                        {
//                            info.Mode_O = false;
//                        }
//                        else
//                        {
//                            continue;
//                        }
//                        break;
//                    case 'o':
//                        if (!umode.Value)
//                        {
//                            info.Mode_o = false;
//                        }
//                        else
//                        {
//                            continue;
//                        }
//                        break;
//                    case 'r':
//                        if (umode.Value)
//                        {
//                            info.Mode_r = true;
//                        }
//                        else
//                        {
//                            continue;
//                        }
//                        break;
//                    case 's':
//                        info.Mode_s = umode.Value;
//                        break;
//                    case 'w':
//                        info.Mode_w = umode.Value;
//                        break;
//                    default:
//                        SendUserModeUnknownFlag(info);
//                        continue;
//                }
//                reply += ((umode.Value) ? "+" : "-") + umode.Key;

//            }
//            if (reply.Length > 0)
//            {
//                SendMode(info, info, info.Nick, reply);
//            }
//        }
//    }
//    else
//    {
//        SendUsersDoNotMatch(info);
//    }
//}
