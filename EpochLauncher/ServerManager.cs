using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Launcher;
using QueryMaster;

namespace EpochLauncher
{
	public class ServerManager
	{
		private ServerBrowser _browser;
		private ConcurrentDictionary<string, IServerInfo> _readyServers;
		private Dispatcher _dispatcher;


		public void JS_RequestUpdate(string handle)
		{
			_dispatcher.InvokeAsync(() => UI_RequestUpdate(handle));
		}

		public void UI_RequestUpdate(string handle)
		{
			
		}

		public ServerManager(Dispatcher mainDispatcher)
		{
			_browser = new ServerBrowser();
			_readyServers = new ConcurrentDictionary<string, IServerInfo>();
			_dispatcher = mainDispatcher;


		}
	}
}
