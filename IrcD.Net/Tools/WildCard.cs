/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2017 Thomas Bruderer <apophis@apophis.ch>
 *  Copyright (c) 2005-2009 Davide Icardi, reinux
 *  
 *  http://www.codeproject.com/KB/recipes/wildcardtoregex.aspx
 *
 *  No explicit license, and GPL 3 in this Project
 */

using System;
using System.Text.RegularExpressions;

namespace IrcD.Tools
{

    /// 
    /// Represents a wildcard running on the
    ///  engine.
    /// 
    public class WildCard : Regex
    {
        /// 
        /// Initializes a wildcard with the given search pattern.
        /// 
        /// The wildcard pattern to match.
        public WildCard(string pattern, WildcardMatch matchType)
            : base(WildcardToRegex(pattern, matchType))
        {
        }

        /// 
        /// Initializes a wildcard with the given search pattern and options.
        /// 
        /// The wildcard pattern to match.
        /// A combination of one or more
        /// .
        public WildCard(string pattern, RegexOptions options, WildcardMatch matchType)
            : base(WildcardToRegex(pattern, matchType), options)
        {
        }

        /// 
        /// Converts a wildcard to a regex.
        /// 
        /// The wildcard pattern to convert.
        /// A regex equivalent of the given wildcard.
        public static string WildcardToRegex(string pattern, WildcardMatch matchType)
        {
            string escapedPattern = Escape(pattern);
            escapedPattern = escapedPattern.Replace("\\*", ".*");
            escapedPattern = escapedPattern.Replace("\\?", ".");

            switch (matchType)
            {
                case WildcardMatch.Exact:
                    return "^" + escapedPattern + "$";
                case WildcardMatch.Anywhere:
                    return escapedPattern;
                case WildcardMatch.StartsWith:
                    return "^" + escapedPattern;
                case WildcardMatch.EndsWith:
                    return escapedPattern + "$";
                default:
                    throw new ArgumentOutOfRangeException("matchType");
            }
        }
    }
}
