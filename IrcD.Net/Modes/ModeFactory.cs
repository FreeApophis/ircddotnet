using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Modes
{
    class ModeFactory
    {
        public void ChangeChannelMode(ref ChannelInfo  channel, string mode)
        {
            
        }

        public void ChangeUserMode(ref UserInfo user, string mode)
        {
            
        }

        public ChannelMode GetChannelMode(char c)
        {
            return null;
        }

        public  UserMode GetUserMode (char c)
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
