using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class OfficialServerInfo
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public OfficialServerInfo(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public IPEndPoint GetEndpoint()
        {
            return new IPEndPoint(IPAddress.Parse(IP), Port);
        }
    }
}
