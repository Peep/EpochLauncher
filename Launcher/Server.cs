using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class Server
    {
        public string Hostname { get; internal set; }
        public int CurrentPlayers { get; internal set; }
        public int MaxPlayers { get; internal set; }
        public int Ping { get; internal set; }
        public bool IsOfficial { get; internal set; }
    }
}
