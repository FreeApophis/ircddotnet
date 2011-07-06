using System;

namespace IrcD
{
    public class RehashEventArgs : EventArgs
    {
        private readonly IrcDaemon ircDaemon;
        public IrcDaemon IrcDaemon
        {
            get
            {
                return ircDaemon;
            }
        }
        private readonly UserInfo userInfo;
        public UserInfo UserInfo
        {
            get
            {
                return userInfo;
            }
        }

        public RehashEventArgs(IrcDaemon ircDaemon, UserInfo userInfo)
        {
            this.ircDaemon = ircDaemon;
            this.userInfo = userInfo;
        }
    }
}
