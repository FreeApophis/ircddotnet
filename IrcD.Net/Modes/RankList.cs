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

using System.Linq;
using System.Text;
using IrcD.Core;
using IrcD.Modes.ChannelRanks;

namespace IrcD.Modes
{
    public class RankList : ModeList<ChannelRank>
    {

        public RankList(IrcDaemon ircDaemon)
            : base(ircDaemon)
        {

        }

        public string ToPrefixList()
        {
            var ranks = new StringBuilder();

            ranks.Append("(");
            foreach (var rank in Values.OrderByDescending(rank => rank.Level))
            {
                ranks.Append(rank.Char);
            }

            ranks.Append(")");

            foreach (var rank in Values.OrderByDescending(rank => rank.Level))
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
                    .OrderByDescending(rank => rank.Value.Level)
                    .Select(rank => rank.Value.Prefix)
                    .DefaultIfEmpty(' ')
                    .First();
            }
        }

        public string NickPrefix => NickPrefixRaw != ' ' ? NickPrefixRaw.ToString() : string.Empty;

        public ChannelRank CurrentRank
        {
            get
            {
                return this
                    .OrderByDescending(rank => rank.Value.Level)
                    .Select(rank => rank.Value)
                    .DefaultIfEmpty(ModeNoRank.Instance)
                    .First();
            }
        }

        public int Level
        {
            get
            {
                return this
                    .OrderByDescending(rank => rank.Value.Level)
                    .Select(rank => rank.Value.Level)
                    .DefaultIfEmpty(0)
                    .First();
            }
        }
    }
}