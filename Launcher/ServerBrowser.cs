using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSQLib;
using Steam.Query;

namespace Launcher
{
    public class ServerBrowser
    {
        private SSQL _query;
	    private MasterServer _masterServer;

        public ServerBrowser()
        {
           _masterServer = new MasterServer();
        }
    }
}
