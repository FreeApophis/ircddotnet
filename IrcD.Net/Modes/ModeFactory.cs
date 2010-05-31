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


using IrcD.Modes.ChannelModes;

namespace IrcD.Modes
{
    class ModeFactory
    {
        public void ChangeChannelMode(ref ChannelInfo channel, string mode)
        {

        }

        public void ChangeUserMode(ref UserInfo user, string mode)
        {

        }

        public ChannelMode GetChannelMode(char c)
        {
            switch (c)
            {
                case 'b': return new ModeBan();
                case 'c': return new ModeColorless();
                case 'e': return new ModeBanException();
                case 'k': return new ModeKey();
                case 'l': return new ModeLimit();
                case 'm': return new ModeModerated();
                case 'n': return new ModeNoExternal();
                case 'p': return new ModePrivate();
                case 's': return new ModeSecret();
                case 't': return new ModeTopic();
                default: return null;
            }
        }

        public UserMode GetUserMode(char c)
        {
            return null;
        }

        //private Dictionary<char, bool> ParseUserMode(string umode)
        //{
        //    var changemodes = new Dictionary<char, bool>();
        //    bool plus = true; // if + or - is ommited at beginning we treat it as +
        //    foreach (char c in umode)
        //    {
        //        switch (c)
        //        {
        //            case '+': plus = true;
        //                break;
        //            case '-': plus = false;
        //                break;
        //            default: changemodes.Add(c, plus);
        //                break;
        //        }
        //    }
        //    return changemodes;
        //}

        //private readonly List<char> modeWithParams = new List<char> { 'b', 'e', 'h', 'I', 'k', 'l', 'o', 'O', 'v' };

        //private class ModeElement
        //{
        //    public ModeElement(char mode, bool? plus, string param)
        //    {
        //        this.mode = mode;
        //        this.plus = plus;
        //        this.param = param;
        //    }

        //    private char mode;

        //    public char Mode
        //    {
        //        get { return mode; }
        //    }

        //    private bool? plus;

        //    public bool? Plus
        //    {
        //        get { return plus; }
        //    }

        //    private string param;

        //    public string Param
        //    {
        //        get { return param; }
        //    }

        //}

        //private List<ModeElement> ParseChannelModes(List<string> cmode)
        //{
        //    var changemodes = new List<ModeElement>();
        //    bool? plus;
        //    int arg = 1;
        //    int paramsNeeded;
        //    while (arg < cmode.Count)
        //    {
        //        plus = null; paramsNeeded = 0;
        //        foreach (char c in cmode[arg])
        //        {
        //            if (c == '+')
        //            {
        //                plus = true;
        //            }
        //            else if (c == '-')
        //            {
        //                plus = false;
        //            }
        //            else
        //            {
        //                if (modeWithParams.Contains(c))
        //                {
        //                    paramsNeeded++;
        //                    if (plus == null)
        //                    {
        //                        changemodes.Add(new ModeElement(c, plus, null));
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            changemodes.Add(new ModeElement(c, plus, cmode[arg + paramsNeeded]));
        //                        }
        //                        catch (ArgumentOutOfRangeException) { }
        //                    }
        //                }
        //                else
        //                {
        //                    changemodes.Add(new ModeElement(c, plus, null));
        //                }

        //            }
        //        }
        //        arg = arg + paramsNeeded + 1;
        //    }

        //    return changemodes;
        //}
    }
}
