/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 *  
 * Copyright (c) 2009-2017, Thomas Bruderer, apophis@apophis.ch All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *   
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * * Neither the name of ArithmeticParser nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Tools
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
