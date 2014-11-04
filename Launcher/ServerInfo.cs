using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class ServerData : QueryMaster.ServerInfo, IServerInfo
    {
        public int CurrentPlayers
        {
            get { return base.Players; }
        }

        public new int MaxPlayers
        {
            get { return base.MaxPlayers; }
        }

        public string Port
        {
            get { return base.Extra.Port.ToString(); }
        }

        public string Handle { get; private set; }

        public new int Ping
        {
            get { return Convert.ToInt32(base.Ping); }
        }
    }
}
