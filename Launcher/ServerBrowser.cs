using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Launcher.Events;
using QueryMaster;

namespace Launcher
{
    public class ServerBrowser
    {
        public Dictionary<int, ServerInfo> Servers { get; internal set; }

        public event EventHandler<ServerAddedEventArgs> ServerAdded;

        public void Refresh()
        {
            if (Servers == null)
                Servers = new Dictionary<int, ServerInfo>();
            else
                Servers.Clear();

            var master = MasterQuery.GetMasterServerInstance(EngineType.Source);
            master.GetAddresses(Region.US_East_coast, ReceiveServers, new IpFilter()
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

            var hash = info.GetHashCode();
            Servers.Add(hash, info);
        }
    }
}
