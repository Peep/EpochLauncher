using System;
using System.Collections.Concurrent;
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

	    public int ServerCount;

        public event EventHandler<ServerEventArgs> ServerAdded;
        public event EventHandler<ServerEventArgs> ServerChanged;

        public static readonly HashSet<IPEndPoint> OfficialServers = new HashSet<IPEndPoint>
			{
				new IPEndPoint(IPAddress.Parse("128.165.214.23"), 2303),
                new IPEndPoint(IPAddress.Parse("5.62.103.29"), 2303),
                new IPEndPoint(IPAddress.Parse("88.198.221.81"), 2303),
                new IPEndPoint(IPAddress.Parse("69.162.70.162"), 2303),
                new IPEndPoint(IPAddress.Parse("69.162.70.162"), 2403),
                new IPEndPoint(IPAddress.Parse("193.192.58.69"), 2303),
                new IPEndPoint(IPAddress.Parse("193.192.58.69"), 2403),
                new IPEndPoint(IPAddress.Parse("192.254.71.234"), 2303),
                new IPEndPoint(IPAddress.Parse("192.254.71.234"), 2403),
                new IPEndPoint(IPAddress.Parse("178.33.137.135"), 2313),
                new IPEndPoint(IPAddress.Parse("192.95.30.52"), 2303),
                new IPEndPoint(IPAddress.Parse("192.99.101.126"), 2303),
                new IPEndPoint(IPAddress.Parse("188.165.250.119"), 2365),
                new IPEndPoint(IPAddress.Parse("62.210.83.13"), 3003),
                new IPEndPoint(IPAddress.Parse("188.165.233.104"), 2503),
                new IPEndPoint(IPAddress.Parse("188.165.233.104"), 2603),
                new IPEndPoint(IPAddress.Parse("192.254.71.233"), 3203),
                new IPEndPoint(IPAddress.Parse("192.254.71.233"), 3303),
                new IPEndPoint(IPAddress.Parse("103.13.103.37"), 2303),
                new IPEndPoint(IPAddress.Parse("178.33.226.75"), 2323),
                new IPEndPoint(IPAddress.Parse("69.162.93.250"), 2702),
			};

        public void Refresh()
        {
	        ServerCount = 0;
            if (Servers == null)
				Servers = new Dictionary<int, ServerInfo>();
            else
                Servers.Clear();

            //var master = MasterQuery.GetMasterServerInstance(EngineType.Source);
            //master.GetAddresses(Region.Rest_of_the_world, ReceiveServers, new IpFilter()
            //{
            //    IsDedicated = true, GameDirectory = "Arma3"
            //});

            foreach (var server in OfficialServers)
                QueryServer(server);
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
            ServerInfo info;

            try
            {
                var server = ServerQuery.GetServerInstance(EngineType.Source, endPoint);
                info = server.GetInfo();
            }
            catch (Exception e)
            {
                return;
            }

            //if (!info.Description.Contains("Epoch"))
            //    return;

            var handle = info.Address.GetHashCode();

            if (Servers.ContainsKey(handle))
            {
				Servers[handle] = info;

                var args = new ServerEventArgs {Handle = handle};
                OnServerChanged(args);
            }
            else
            {
	            Servers.Add(handle, info);

                var args = new ServerEventArgs {Handle = handle};
                OnServerAdded(args);
            }
        }

        protected virtual void OnServerAdded(ServerEventArgs e)
        {
	        ServerCount++;
            var handler = ServerAdded;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnServerChanged(ServerEventArgs e)
        {
            var handler = ServerChanged;
            if (handler != null)
                handler(this, e);
        }

        public enum VerifiedOnly
        {
            Optional,
            Required
        };
    }
}
