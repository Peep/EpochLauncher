using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private const long MAX_QUERIES = 1000;
        private static long _currentNumberOfQueries;

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
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadStringCompleted += async (s, args) =>
                        await Task.Run(() => ReceiveServers(args.Result));
                    wc.DownloadStringAsync(new Uri("https://launcher.bmrf.me/servers.json"));
                }
            }
            else
            {
                var master = MasterQuery.GetMasterServerInstance(EngineType.Source);
                master.GetAddresses(Region.Rest_of_the_world, ReceiveServers, new IpFilter
                {
                    IsDedicated = true,
                    GameDirectory = "Arma3"
                });
            }
        }

        public void Refresh(int serverHandle)
        {
            if (!Servers.ContainsKey(serverHandle)) return;

            var address = Servers[serverHandle].Address.Split(':');
            QueryServer(new IPEndPoint(IPAddress.Parse(address[0]), Convert.ToInt32(address[1])));
        }

        void ReceiveServers(ReadOnlyCollection<IPEndPoint> endPoints)
        {
            foreach (var endPoint in endPoints.Where(ip => ip.Address.ToString() != "0.0.0.0"))
                QueryServerAsync(endPoint);
        }

        void ReceiveServers(string json)
        {
            OfficialServers = JsonConvert.DeserializeObject<HashSet<OfficialServerInfo>>(json);
            foreach (var server in OfficialServers)
                QueryServerAsync(server.GetEndpoint());
        }

        async void QueryServerAsync(IPEndPoint endPoint)
        {
            if (_currentNumberOfQueries > MAX_QUERIES)
            {
                var wait = new SpinWait();
                while (_currentNumberOfQueries > MAX_QUERIES) wait.SpinOnce();
            }

            await Task.Run(() => { Interlocked.Increment(ref _currentNumberOfQueries); QueryServer(endPoint); });
        }

        void QueryServer(IPEndPoint endPoint)
        {
            try
            {
                var server = ServerQuery.GetServerInstance(EngineType.Source, endPoint);
                var info = server.GetInfo();

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
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                Interlocked.Decrement(ref _currentNumberOfQueries);
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
    }
}
