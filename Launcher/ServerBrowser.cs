using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QueryMaster;

namespace Launcher
{
    public class ServerBrowser
    {
        private MasterServer _master;

        public ServerBrowser()
        {
            _master = MasterQuery.GetMasterServerInstance(EngineType.Source);
            _master.GetAddresses(Region.US_East_coast, ReceiveServers);
        }

        void ReceiveServers(ReadOnlyCollection<IPEndPoint> endPoints)
        {
            foreach (var ip in endPoints)
            {
                //"0.0.0.0:0" is the last address 
                if (ip.Address.ToString() != "0.0.0.0")
                    QueryServer(ip);
            }
        }

        void QueryServer(IPEndPoint endPoint)
        {
            
        }
    }
}
