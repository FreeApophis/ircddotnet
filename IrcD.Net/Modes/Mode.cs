using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Modes
{
    class Mode
    {
        private char mode;

        public char Mode
        {
            get
            {
                return mode;
            }
        }

        public Mode(char mode)
        {
            this.mode = mode;
        }
    }
}
