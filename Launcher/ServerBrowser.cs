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

        public event EventHandler<ServerEventArgs> ServerAdded;
        public event EventHandler<ServerEventArgs> ServerChanged;

        public void Refresh()
        {
            if (Servers == null)
                Servers = new Dictionary<int, ServerInfo>();
            else
                Servers.Clear();

            var master = MasterQuery.GetMasterServerInstance(EngineType.Source);
            master.GetAddresses(Region.Rest_of_the_world, ReceiveServers, new IpFilter()
            {
                IsDedicated = true, GameDirectory = "Arma3"
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

            if (!info.Description.Contains("Epoch"))
                return;

            var handle = info.Address.GetHashCode();

            if (Servers.ContainsKey(handle))
            {
                lock (Servers)
                    Servers[handle] = info;

                var args = new ServerEventArgs {Handle = handle};
                OnServerChanged(args);
            }
            else
            {
                lock (Servers)
                    Servers.Add(handle, info);

                var args = new ServerEventArgs {Handle = handle};
                OnServerAdded(args);
            }
        }

        protected virtual void OnServerAdded(ServerEventArgs e)
        {
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
