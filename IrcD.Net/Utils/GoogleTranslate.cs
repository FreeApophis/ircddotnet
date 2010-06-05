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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace IrcD.Utils
{
    public class GoogleTranslate
    {
        private readonly Dictionary<string, string> languages = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
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

        public Dictionary<string, string> Languages
        {
            get
            {
                return languages;
            }
        }

        public string TranslateText(string input, string sourceLanguage, string targetLanguage)
        {
            var url = String.Format("http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q={0}&langpair={1}", input, sourceLanguage + "|" + targetLanguage);

            var webClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            var result = webClient.DownloadString(url);
            var jsonObj = JSON.JsonDecode(result);
            if (jsonObj is Hashtable)
            {
                if (((Hashtable)jsonObj)["responseData"] == null)
                {
                    throw new Exception((string)((Hashtable)jsonObj)["responseDetails"]);
                }
                return HttpUtility.HtmlDecode(((string)((Hashtable)((Hashtable)jsonObj)["responseData"])["translatedText"]));
            }
            return null;
        }

        public string DetectLanguage(string input)
        {
            var url = String.Format("http://ajax.googleapis.com/ajax/services/language/detect?v=1.0&q={0}", input);
            var webClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            var result = webClient.DownloadString(url);
            var jsonObj = JSON.JsonDecode(result);

            return jsonObj is Hashtable ? (string)((Hashtable)((Hashtable)jsonObj)["responseData"])["language"] : null;
        }
    }
}
