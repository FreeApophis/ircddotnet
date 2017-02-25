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

namespace IrcD.Tools
{
    public class Languages
    {
        public static Dictionary<string, string> All { get; } = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            { "af", "Afrikaans" },
            { "sq", "Albanian" },
            { "am", "Amharic" },
            { "ar", "Arabic" },
            { "hy", "Armenian" },
            { "az", "Azerbaijani" },
            { "eu", "Basque" },
            { "be", "Belarusian" },
            { "bn", "Bengali" },
            { "bh", "Bihari" },
            { "bg", "Bulgarian" },
            { "my", "Burmese" },
            { "ca", "Catalan" },
            { "chr", "Cherokee" },
            { "zh", "Chinese" },
            { "zh-CN", "Simplified Chinese" },
            { "zh-TW", "Traditional Chinese" },
            { "hr", "Croatian" },
            { "cs", "Czech" },
            { "da", "Danish" },
            { "dv", "Dhivehi" },
            { "nl", "Dutch" },
            { "en", "English" },
            { "eo", "Esperanto" },
            { "et", "Estonian" },
            { "tl", "Filipino" },
            { "fi", "Finnish" },
            { "fr", "French" },
            { "gl", "Galician" },
            { "ka", "Georgian" },
            { "de", "German" },
            { "el", "Greek" },
            { "gn", "Guarani" },
            { "gu", "Gujarati" },
            { "iw", "Hebrew" },
            { "hi", "Hindi" },
            { "hu", "Hungarian" },
            { "is", "Icelandic" },
            { "id", "Indonesian" },
            { "iu", "Inuktitut" },
            { "it", "Italian" },
            { "ja", "Japanese" },
            { "kn", "Kannada" },
            { "kk", "Kazakh" },
            { "km", "Khmer" },
            { "ko", "Korean" },
            { "ku", "Kurdish" },
            { "ky", "Kyrgyz" },
            { "lo", "Laothian" },
            { "lv", "Latvian" },
            { "lt", "Lithuanian" },
            { "mk", "Macedonian" },
            { "ms", "Malay" },
            { "ml", "Malayalam" },
            { "mt", "Maltese" },
            { "mr", "Marathi" },
            { "mn", "Mongolian" },
            { "ne", "Nepali" },
            { "no", "Norwegian" },
            { "or", "Oriya" },
            { "ps", "Pashto" },
            { "fa", "Persian" },
            { "pl", "Polish" },
            { "pt-PT", "Portuguese" },
            { "pa", "Punjabi" },
            { "ro", "Romanian" },
            { "ru", "Russian" },
            { "sa", "Sanskrit" },
            { "sr", "Serbian" },
            { "sd", "Sindhi" },
            { "si", "Sinhalese" },
            { "sk", "Slovak" },
            { "sl", "Slovenian" },
            { "es", "Spanish" },
            { "sw", "Swahili" },
            { "sv", "Swedish" },
            { "tg", "Tajik" },
            { "ta", "Tamil" },
            /*          { "tl", "Tagalog" }, == FILIPINO */
            { "te", "Telugu" },
            { "th", "Thai" },
            { "bo", "Tibetan" },
            { "tr", "Turkish" },
            { "uk", "Ukrainian" },
            { "ur", "Urdu" },
            { "uz", "Uzbek" },
            { "ug", "Uighur" },
            { "vi", "Vietnamese" },
        };
    }
}
