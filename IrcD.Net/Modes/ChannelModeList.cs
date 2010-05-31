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

using System.Linq;
using System.Text;
using IrcD.Modes.ChannelModes;

namespace IrcD.Modes
{
    public class ChannelModeList : ModeList<ChannelMode>
    {
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
    }
}