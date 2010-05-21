using System.Text;
using IrcD.Modes;

namespace IrcD
{
    public abstract class InfoBase
    {
        private readonly IrcDaemon ircDaemon;

        public IrcDaemon IrcDaemon
        {
            get
            {
                return ircDaemon;
            }
        }

        protected InfoBase(IrcDaemon ircDaemon)
        {
            this.ircDaemon = ircDaemon;
        }

        /// <summary>
        /// Write a Line to the abstract object (hide the socket better)
        /// </summary>
        public abstract void WriteLine(StringBuilder line);
    }
}