using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Launcher;
using Launcher.Events;
using QueryMaster;

namespace EpochLauncher
{
	public class ServerManager
		: IServerStore
	{
		private readonly ServerBrowser _browser;
		private readonly ConcurrentDictionary<string, IServerInfo> _readyServers;
		private readonly Dispatcher _dispatcher;

		public IServerInfo JS_RequestInfo(string handle)
		{
			var info = Find(handle);
			JS_RequestUpdate(handle);
			return info;
		}

		public void JS_RequestUpdate(string handle)
		{
			_dispatcher.InvokeAsync(() => UI_RequestUpdate(handle));
		}

		public void UI_RequestUpdate(string handle)
		{
			var info = Find(handle);
			_browser.Refresh(info);
		}

		public ServerManager(Dispatcher mainDispatcher)
		{
			_browser = new ServerBrowser();
			_readyServers = new ConcurrentDictionary<string, IServerInfo>();
			_dispatcher = mainDispatcher;

			_browser.ServerAdded += BrowserOnServerAdded;
			_browser.ServerChanged += BrowserOnServerChanged;

		}

		private void BrowserOnServerChanged(object sender, ServerEventArgs serverEventArgs)
		{
			
		}

		private void BrowserOnServerAdded(object sender, ServerEventArgs serverEventArgs)
		{
			var info = new ServerInfo(serverEventArgs.Server);
		}

		public IServerInfo Find(string jsHandle)
		{
			IServerInfo info;
			_readyServers.TryGetValue(jsHandle, out info);
			return info;
		}

		public int ServerCount
		{
			get { return _readyServers.Count; }
		}

		public void Refresh(string handle)
		{
			UI_RequestUpdate(handle);
		}

		public void Refresh()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IServerInfo> ServerList
		{
			get { return _readyServers.Values; }
		}
	}
}
