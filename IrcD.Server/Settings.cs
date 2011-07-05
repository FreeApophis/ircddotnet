using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace IrcD.Server
{
    class Settings
    {
        private IrcDaemon ircDaemon;
        private XDocument configFile;
        public Settings()
        {
            var readerSettings = new XmlReaderSettings();
            readerSettings.CheckCharacters = false;
            var reader = XmlReader.Create("config.xml", readerSettings);

            configFile = XDocument.Load(reader);
        }

        public void LoadSettings()
        {
            ircDaemon.Options.ClientCompatibilityMode = GetBool("client_compatibility", true);
            ircDaemon.Options.IrcCaseMapping = GetCaseMapping();
            ircDaemon.Options.MaxAwayLength = GetInt("max_away_length", 300);
            ircDaemon.Options.MaxChannelLength = GetInt("max_channel_length", 50);
            ircDaemon.Options.MaxKickLength = GetInt("max_kick_length", 300);
            ircDaemon.Options.MaxLanguages = GetInt("max_languages", 5);
            ircDaemon.Options.MaxLineLength = GetInt("max_line_length", 510);
            ircDaemon.Options.MaxNickLength = GetInt("max_nick_length", 9);
            ircDaemon.Options.MaxSilence = GetInt("max_silence", 20);
            ircDaemon.Options.MaxTopicLength = GetInt("max_topic_length", 300);
            ircDaemon.Options.MessageOfTheDay = GetString("motd", null);
            ircDaemon.Options.NetworkName = GetString("network_name", null);
            ircDaemon.Options.ServerName = GetString("server_name", null);
            ircDaemon.Options.ServerPass = GetString("server_pass", null);
            ircDaemon.Options.ServerPorts = GetPorts();
            ircDaemon.Options.StandardKickMessage = GetString("standard_kick_message", "Kicked");
            ircDaemon.Options.StandardPartMessage = GetString("standard_part_message", "Leaving");
            ircDaemon.Options.StandardQuitMessage = GetString("standard_quit_message", "Quit");
        }

        private List<int> GetPorts()
        {
            List<int> result = new List<int>();
            foreach (var iface in configFile.Descendants("interface"))
            {
                if (iface.Element("port") != null)
                {
                    int port;
                    if (int.TryParse(iface.Element("port").Value, out port))
                    {
                        result.Add(port);
                    }
                }
            }

            if (!result.Any())
            {
                result.Add(6667);
            }

            return result;
        }

        private IrcCaseMapping GetCaseMapping()
        {
            var mode = configFile.Descendants("case_mapping").Select(m => m.Value).FirstOrDefault();

            if (mode == default(string))
                return IrcCaseMapping.Ascii;

            IrcCaseMapping result;
            if (Enum.TryParse(mode, true, out result))
            {
                return result;
            }

            return IrcCaseMapping.Ascii;

        }

        private bool GetBool(string key, bool standard)
        {
            bool result;
            var match = configFile.Descendants(key);

            if (match.Any())
            {
                if (bool.TryParse(match.First().Value, out result))
                {
                    return result;
                }
            }

            return standard;
        }

        private int GetInt(string key, int standard)
        {
            int result;
            var match = configFile.Descendants(key);

            if (match.Any())
            {
                if (int.TryParse(match.First().Value, out result))
                {
                    return result;
                }
            }

            return standard;
        }

        private string GetString(string key, string standard)
        {
            var match = configFile.Descendants(key);

            if (match.Any())
            {
                return match.First().Value;
            }

            return standard;
        }

        internal IrcMode GetIrcMode()
        {
            var mode = configFile.Descendants("irc_mode").Select(m => m.Value).FirstOrDefault();

            if (mode == default(string))
                return IrcMode.Modern;

            IrcMode result;
            if (Enum.TryParse(mode, true, out result))
            {
                return result;
            }

            return IrcMode.Modern;
        }

        internal void setDaemon(IrcDaemon ircDaemon)
        {
            this.ircDaemon = ircDaemon;
        }
    }
}
