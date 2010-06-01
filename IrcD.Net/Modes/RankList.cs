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

namespace IrcD.Modes
{
    public class RankList : ModeList<ChannelRank>
    {
        public string ToPrefixList()
        {
            var ranks = new StringBuilder();

            ranks.Append("(");
            foreach (var rank in Values.OrderByDescending(rank => rank.Importance))
            {
                ranks.Append(rank.Char);
            }
            ranks.Append(")");

            foreach (var rank in Values.OrderByDescending(rank => rank.Importance))
            {
                ranks.Append(rank.Prefix);
            }

            return ranks.ToString();
        }

        public char NickPrefixRaw
        {
            get
            {
                return this
                    .OrderByDescending(rank => rank.Value.Importance)
                    .Select(rank => rank.Value.Char)
                    .DefaultIfEmpty(' ')
                    .First();
            }
        }

        public string NickPrefix
        {
            get
            {
                return NickPrefixRaw != ' ' ? NickPrefixRaw.ToString() : "";
            }
        }
    }
}