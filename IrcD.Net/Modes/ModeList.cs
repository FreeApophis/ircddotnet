using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Modes
{
    class ModeList<T> : SortedList<char, T> where T : Mode
    {
        public void Add(T element)
        {
            this.Add(element.Mode, element);
        }

        public override string ToString()
        {
            StringBuilder modes = new StringBuilder();
            
            foreach (var mode in this) 
            {
                modes.Append(mode.Value.Mode);
            }

            return modes.ToString();
        }
    }
}
