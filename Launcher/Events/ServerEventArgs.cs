using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryMaster;

namespace Launcher.Events
{
    public class ServerEventArgs
    {
        public string Handle { get; internal set; }
        public ServerInfo Server { get; internal set; }
    }
}
