using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Commands
{
    public class CheckParamCountAttribute : Attribute
    {
        readonly int minimumParameterCount;
        public int MinimumParameterCount
        {
            get
            {
                return minimumParameterCount;
            }
        }

        public CheckParamCountAttribute(int minimumParameterCount)
        {
            this.minimumParameterCount = minimumParameterCount;
        }
    }
}
