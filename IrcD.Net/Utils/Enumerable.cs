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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Utils
{
    public static class Enumerable
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        {
            var ie1 = first.GetEnumerator();
            var ie2 = second.GetEnumerator();

            while (ie1.MoveNext() && ie2.MoveNext())
                yield return func(ie1.Current, ie2.Current);
        }

        public static string Concatenate<T>(this IEnumerable<T> strings, string separator)
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in strings)
            {
                stringBuilder.Append(item);
                stringBuilder.Append(separator);
            }

            stringBuilder.Length = stringBuilder.Length - separator.Length;
            return stringBuilder.ToString();
        }

        public static IEnumerable<EnumerableIndex<T>> EachIndex<T>(this IEnumerable<T> collection, int index = 0)
        {
            return collection.Select(value => new EnumerableIndex<T> { Value = value, Index = index++ });
        }
    }
}
