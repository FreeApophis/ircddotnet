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

namespace IrcD.Modes.ChannelRanks
{
    public class ModeVoice : ChannelRank
    {
        public const int VoiceLevel = 10;

        public ModeVoice()
            : base('v', '+', VoiceLevel)
        {

        }

        public override bool CanChangeChannelMode(ChannelMode mode) => false;
        public override bool CanChangeChannelRank(ChannelRank rank) => false;
    }
}
