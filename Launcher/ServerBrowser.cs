using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Launcher.Events;
using Newtonsoft.Json;
using QueryMaster;

namespace Launcher
{
    public class ServerBrowser
    {
        public Dictionary<int, ServerInfo> Servers { get; internal set; }
        public HashSet<OfficialServerInfo> OfficialServers { get; internal set; }

	    public int ServerCount;

        public event EventHandler<ServerEventArgs> ServerAdded;
        public event EventHandler<ServerEventArgs> ServerChanged;

        public void Refresh(bool verifiedOnly = true)
        {
            if (OfficialServers == null)
                OfficialServers = new HashSet<OfficialServerInfo>();

            ServerCount = 0;
            if (Servers == null)
                Servers = new Dictionary<int, ServerInfo>();
            else
                Servers.Clear();

            if (verifiedOnly)
                using (var wc = new WebClient())
                    OfficialServers = JsonConvert.DeserializeObject<HashSet<OfficialServerInfo>>
                        (wc.DownloadString("https://launcher.bmrf.me/servers.json"));
            else
            {
                var master = MasterQuery.GetMasterServerInstance(EngineType.Source);
                master.GetAddresses(Region.Rest_of_the_world, ReceiveServers, new IpFilter()
                {
                    IsDedicated = true,
                    GameDirectory = "Arma3"
                });
            }

            foreach (var server in OfficialServers)
                QueryServer(server.GetEndpoint());
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
