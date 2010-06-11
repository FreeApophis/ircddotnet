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
using System.Linq;
using System.Net;
using System.Web;

namespace IrcD.Utils
{
    public class GoogleTranslate
    {
        private static readonly Dictionary<string, string> languages = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
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

        public static Dictionary<string, string> Languages
        {
            get
            {
                return languages;
            }
        }

        public delegate Tuple<string, string> TranslateDelegate(string input, string targetLanguage, string sourceLanguage = "");
        public delegate Dictionary<string, Tuple<string, string, string>> TranslateMultipleDelegate(string input, IEnumerable<string> targetLanguages);

        public Tuple<string, string> TranslateText(string input, string targetLanguage, string sourceLanguage = "")
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
                return new Tuple<string, string>(HttpUtility.HtmlDecode(((string)((Hashtable)((Hashtable)jsonObj)["responseData"])["translatedText"])), HttpUtility.HtmlDecode(((string)((Hashtable)((Hashtable)jsonObj)["responseData"])["detectedSourceLanguage"])));
            }
            return null;
        }

        public Dictionary<string, Tuple<string, string, string>> TranslateText(string input, IEnumerable<string> targetLanguages)
        {
            var result = new Dictionary<string, Tuple<string, string, string>>();

            try
            {
                var url = String.Format("http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q={0}&langpair=%7C{1}", input, targetLanguages.Concatenate("&langpair=%7C"));
                var webClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };
                var resultString = webClient.DownloadString(url);
                var jsonObj = JSON.JsonDecode(resultString);

                var allResponses = jsonObj as Hashtable;
                if (allResponses != null)
                {
                    var responseData = allResponses["responseData"] as ArrayList;
                    if (responseData != null)
                    {
                        foreach (var singleResponse in responseData.OfType<Hashtable>().Zip(targetLanguages, (f, s) => new { Translation = f, Target = s }))
                        {
                            var singleResponseData = singleResponse.Translation["responseData"] as Hashtable;
                            string text;
                            string sourcelang;
                            if (singleResponseData != null)
                            {
                                text = HttpUtility.HtmlDecode(singleResponseData["translatedText"] as string);
                                sourcelang = HttpUtility.HtmlDecode(singleResponseData["detectedSourceLanguage"] as string);
                            }
                            else
                            {
                                text = input;
                                sourcelang = "F";
                            }
                            result.Add(singleResponse.Target, new Tuple<string, string, string>(sourcelang, singleResponse.Target, text));
                        }
                    }
                    else
                    {
                        var singleResponseData = allResponses["responseData"] as Hashtable;

                        string text;
                        string sourcelang;
                        if (singleResponseData != null)
                        {
                            text = HttpUtility.HtmlDecode(singleResponseData["translatedText"] as string);
                            sourcelang = HttpUtility.HtmlDecode(singleResponseData["detectedSourceLanguage"] as string);
                        }
                        else
                        {
                            text = input;
                            sourcelang = "F";
                        }
                        result.Add(targetLanguages.First(), new Tuple<string, string, string>(sourcelang, targetLanguages.First(), text));
                    }
                }
            }
            finally
            {
                if (result.Count == 0)
                {
                    foreach (var targetLanguage in targetLanguages)
                    {
                        result.Add(targetLanguage, new Tuple<string, string, string>("F", targetLanguage, input));
                    }
                }
            }
            return result;
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
