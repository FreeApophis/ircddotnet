/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009 Thomas Bruderer <apophis@apophis.ch>
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


namespace IrcD
{
    class UserPerChannelInfo
    {
        public UserPerChannelInfo(UserInfo info)
        {
            this.info = info;
        }

        private UserInfo info;

        public UserInfo Info
        {
            get
            {
                return info;
            }
        }

        public string NickPrefix
        {
            get
            {
                if (mode_o)
                {
                    return "@";
                }
                if (mode_h)
                {
                    return "%";
                }
                if (mode_v)
                {
                    return "+";
                }
                return "";
            }
        }

        private bool mode_v;

        /// <summary>
        /// Is User Voiced
        /// </summary>
        public bool Mode_v
        {
            get
            {
                return mode_v;
            }
            set
            {
                mode_v = value;
            }
        }

        private bool mode_h;

        /// <summary>
        /// Is User Half-Op
        /// </summary>
        public bool Mode_h
        {
            get
            {
                return mode_h;
            }
            set
            {
                mode_h = value;
            }
        }


        private bool mode_o;

        /// <summary>
        /// Is User Op
        /// </summary>
        public bool Mode_o
        {
            get
            {
                return mode_o;
            }
            set
            {
                mode_o = value;
            }
        }
    }
}
