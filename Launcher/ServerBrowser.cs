using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using SSQLib;
using Steam.Query;
>>>>>>> origin/dev

namespace Launcher
{
    public class ServerBrowser
    {
<<<<<<< HEAD

        public ServerBrowser()
        {
=======
        private SSQL _query;
	    private MasterServer _masterServer;

        public ServerBrowser()
        {
			_masterServer = new MasterServer("hl2master.steampowered.com");
>>>>>>> origin/dev
        }
    }
}
