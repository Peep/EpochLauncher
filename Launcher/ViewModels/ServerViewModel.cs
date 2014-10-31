using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ViewModels
{
    public class ServerViewModel
    {
        public string Hostname { get; internal set; }
        public string IpAddress { get; internal set; }
        public int Port { get; internal set; }
        public string Game { get; internal set; }
        public string Mods { get; internal set; }
        public string Map { get; internal set; }
        public int CurrentPlayers { get; internal set; }
        public int MaxPlayers { get; internal set; }
        public int Ping { get; internal set; }
    }
}
