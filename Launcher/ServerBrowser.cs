using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class ServerBrowser
    {
    }

    public class Server
    {
        public string Hostname { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int? Ping { get; set; }
    }
}
