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
    /// <summary>
    /// Emulates the Tuple from the System Namespace for the non .NET 4.0 Build.
    /// </summary>
    public class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 t1)
        {
            return new Tuple<T1>(t1);
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new Tuple<T1, T2>(t1, t2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Tuple<T1, T2, T3>(t1, t2, t3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }
    }

    public class Tuple<T1>
    {
        public Tuple(T1 t1)
        {
            Item1 = t1;
        }

        public T1 Item1 { get; private set; }
    }

    public class Tuple<T1, T2>
    {
        public Tuple(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
    }

    public class Tuple<T1, T2, T3>
    {
        public Tuple(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
    }

    public class Tuple<T1, T2, T3, T4>
    {
        public Tuple(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
            Item4 = t4;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
    }
}
