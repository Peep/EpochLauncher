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

        public void Refresh(int serverHandle)
        {
            if (!Servers.ContainsKey(serverHandle)) return;

            var address = Servers[serverHandle].Address.Split(':');
            QueryServer(new IPEndPoint(IPAddress.Parse(address[0]), Convert.ToInt32(address[1])));
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
            var handle = info.Address.GetHashCode();

            if (Servers.ContainsKey(handle))
            {
                Servers[handle] = info;
                // Call object modified event
            }
            else
            {
                Servers.Add(handle, info);
                // Call object added event
            }
        }
    }
}
