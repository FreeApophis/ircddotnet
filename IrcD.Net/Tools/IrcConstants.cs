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


namespace IrcD.Utils
{
    public class IrcConstants
    {
        public const char CtcpChar = '\x1';
        public const char IrcBold = '\x2';
        public const char IrcColor = '\x3';
        public const char IrcReverse = '\x16';
        public const char IrcNormal = '\xf';
        public const char IrcUnderline = '\x1f';
        public const char CtcpQuoteChar = '\x20';

    }
}
