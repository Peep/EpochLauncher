using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Launcher.ViewModels;
using QueryMaster;

namespace Launcher
{
    public class ServerBrowser
    {
        public List<ServerInfo> Servers { get; internal set; } 
        private MasterServer _master;

        public ServerBrowser()
        {
            Servers = new List<ServerInfo>();
            _master = MasterQuery.GetMasterServerInstance(EngineType.Source);
            _master.GetAddresses(Region.US_East_coast, ReceiveServers, new IpFilter()
            {
                IsDedicated = true
            });
        }

        void ReceiveServers(ReadOnlyCollection<IPEndPoint> endPoints)
        {
            foreach (var ip in endPoints.Where(ip => ip.Address.ToString() != "0.0.0.0"))
                QueryServer(ip);
        }

        void QueryServer(IPEndPoint endPoint)
        {
            var server = ServerQuery.GetServerInstance(EngineType.Source, endPoint);
            var info = server.GetInfo();
            Servers.Add(info);
        }
    }
}
