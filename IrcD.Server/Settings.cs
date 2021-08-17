using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using IrcD.Core;
using IrcD.Core.Utils;
using IrcD.Tools;

namespace IrcD.Server
{
    class Settings
    {
        private IrcDaemon _ircDaemon;
        private readonly XDocument _configFile;
        public Settings()
        {
            var readerSettings = new XmlReaderSettings { CheckCharacters = false };

            using (var reader = XmlReader.Create("config.xml", readerSettings))
            {
                _configFile = XDocument.Load(reader);
            }

        }

        public void LoadSettings()
        {
            _ircDaemon.Options.ClientCompatibilityMode = GetBool("client_compatibility", true);
            _ircDaemon.Options.IrcCaseMapping = GetCaseMapping();
            _ircDaemon.Options.MaxAwayLength = GetInt("max_away_length", 300);
            _ircDaemon.Options.MaxChannelLength = GetInt("max_channel_length", 50);
            _ircDaemon.Options.MaxKickLength = GetInt("max_kick_length", 300);
            _ircDaemon.Options.MaxLanguages = GetInt("max_languages", 5);
            _ircDaemon.Options.MaxLineLength = GetInt("max_line_length", 510);
            _ircDaemon.Options.MaxNickLength = GetInt("max_nick_length", 9);
            _ircDaemon.Options.MaxSilence = GetInt("max_silence", 20);
            _ircDaemon.Options.MaxTopicLength = GetInt("max_topic_length", 300);
            _ircDaemon.Options.MessageOfTheDay = GetString("motd", null);
            _ircDaemon.Options.NetworkName = GetString("network_name", null);
            _ircDaemon.Options.ServerName = GetString("server_name", null);
            _ircDaemon.Options.ServerPass = GetString("server_pass", null);
            _ircDaemon.Options.ServerPorts = GetPorts();
            _ircDaemon.Options.StandardKickMessage = GetString("standard_kick_message", "Kicked");
            _ircDaemon.Options.StandardPartMessage = GetString("standard_part_message", "Leaving");
            _ircDaemon.Options.StandardQuitMessage = GetString("standard_quit_message", "Quit");
            _ircDaemon.Options.StandardKillMessage = GetString("standard_kill_message", "Killed");

            _ircDaemon.Options.AdminLocation1 = GetString("admin", "location1", "no admin set");
            _ircDaemon.Options.AdminLocation2 = GetString("admin", "location2", "no admin set");
            _ircDaemon.Options.AdminEmail = GetString("admin", "email", "no admin set");

            LoadOper();
            LoadOperHosts();
        }

        private void LoadOper()
        {
            foreach (var oper in _configFile.Descendants("oper"))
            {
                if (!string.IsNullOrEmpty(oper.Element("user")?.Value) && !string.IsNullOrEmpty(oper.Element("pass")?.Value))
                {
                    _ircDaemon.Options.OLine.Add(oper.Element("user")?.Value, oper.Element("pass")?.Value);
                }
            }
        }

        private void LoadOperHosts()
        {
            foreach (var host in _configFile.Descendants("oper_hosts"))
            {
                foreach (var entry in host.Elements())
                {
                    switch (entry.Name.LocalName)
                    {
                        case "allow":
                            _ircDaemon.Options.OperHosts.Add(new OperHost { Allow = true, WildcardHostMask = new WildCard(entry.Value, WildcardMatch.Anywhere) });
                            break;
                        case "deny":
                            _ircDaemon.Options.OperHosts.Add(new OperHost { Allow = false, WildcardHostMask = new WildCard(entry.Value, WildcardMatch.Anywhere) });
                            break;
                    }
                }
            }
        }

        private List<int> GetPorts()
        {
            List<int> result = new List<int>();
            foreach (var iface in _configFile.Descendants("interface"))
            {
                if (iface.Element("port") != null)
                {
                    if (int.TryParse(iface.Element("port")?.Value, out int port))
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
            var mode = _configFile.Descendants("case_mapping").Select(m => m.Value).FirstOrDefault();

            if (mode == default(string))
                return IrcCaseMapping.Ascii;

            if (Enum.TryParse(mode, true, out IrcCaseMapping result))
            {
                return result;
            }

            return IrcCaseMapping.Ascii;

        }

        private bool GetBool(string key, bool standard)
        {
            var match = _configFile.Descendants(key).ToList();

            if (match.Any())
            {
                if (bool.TryParse(match.First().Value, out bool result))
                {
                    return result;
                }
            }

            return standard;
        }

        private int GetInt(string key, int standard)
        {
            var match = _configFile.Descendants(key).ToList();

            if (match.Any())
            {
                if (int.TryParse(match.First().Value, out int result))
                {
                    return result;
                }
            }

            return standard;
        }

        private string GetString(string key, string standard)
        {
            var match = _configFile.Descendants(key).ToList();

            if (match.Any())
            {
                return match.First().Value;
            }

            return standard;
        }

        private string GetString(string key, string specifier, string standard)
        {
            var match = _configFile.Descendants(key).ToList();

            if (match.Any())
            {
                var element = match.First().Element(specifier);
                if (!string.IsNullOrEmpty(element?.Value))
                {
                    return element.Value;
                }
            }

            return standard;
        }

        internal IrcMode GetIrcMode()
        {
            var mode = _configFile.Descendants("irc_mode").Select(m => m.Value).FirstOrDefault();

            if (mode == default(string))
                return IrcMode.Modern;

            if (Enum.TryParse(mode, true, out IrcMode result))
            {
                return result;
            }

            return IrcMode.Modern;
        }

        internal void SetDaemon(IrcDaemon ircDaemon)
        {
            _ircDaemon = ircDaemon;
        }
    }
}
