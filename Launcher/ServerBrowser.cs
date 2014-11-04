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
	    public int ServerCount;
        private const long MAX_QUERIES = 1000;
        private static long _currentNumberOfQueries;

        public event EventHandler<ServerEventArgs> ServerAdded;
        public event EventHandler<ServerEventArgs> ServerChanged;

        public void Refresh(bool verifiedOnly = false)
        {
            ServerCount = 0;

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
                    //GameDirectory = "Arma3"
                });
            }
        }

        public void Refresh(IServerInfo info)
        {          
            var address = info.Address.Split(':');
            QueryServer(new IPEndPoint(IPAddress.Parse(address[0]), Convert.ToInt32(address[1])));
        }

        void ReceiveServers(ReadOnlyCollection<IPEndPoint> endPoints)
        {
            Console.WriteLine("TOTAL SERVERS TO QUERY:" + endPoints.Count);
            foreach (var endPoint in endPoints.Where(ip => ip.Address.ToString() != "0.0.0.0"))        
                QueryServerAsync(endPoint);
        }

        void ReceiveServers(string json)
        {
            var servers = JsonConvert.DeserializeObject<HashSet<OfficialServerInfo>>(json);
            foreach (var server in servers)
                QueryServerAsync(server.GetEndpoint());
        }

        async void QueryServerAsync(IPEndPoint endPoint)
        {
            if (_currentNumberOfQueries > MAX_QUERIES)
            {
                var wait = new SpinWait();
                while (_currentNumberOfQueries > MAX_QUERIES) wait.SpinOnce();
            }
            Console.WriteLine("THREADS:" + _currentNumberOfQueries);
            await Task.Run(() => { Interlocked.Increment(ref _currentNumberOfQueries); QueryServer(endPoint); });
        }

        void QueryServer(IPEndPoint endPoint)
        {
            try
            {
                var server = ServerQuery.GetServerInstance(EngineType.Source, endPoint);
                var info = server.GetInfo();

                var handle = String.Format("{0}:{1}", info.Address, info.Extra.Port);

                var args = new ServerEventArgs { Handle = handle, Server = info };
                OnServerAdded(args);
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
