using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		private readonly ConcurrentDictionary<string, ServerInfo> _readyServers;
		private readonly Dispatcher _dispatcher;


		private List<ServerInfo> _sortedServers; 

		public IServerInfo JS_RequestServer(string handle)
		{
			var info = Find(handle);
			JS_RequestUpdate(handle);
			return info;
		}

		public void JS_RequestUpdate(string handle)
		{
			_dispatcher.InvokeAsync(() => UI_RequestUpdate(handle));
		}

		public void JS_RequestRefresh()
		{
			_dispatcher.InvokeAsync(UI_RequestRefresh);
		}

		public void UI_RequestUpdate(string handle)
		{
			var info = Find(handle);
			if (info == null)
				return;

			_browser.Refresh(info);
		}

		public void UI_RequestUpdate(ServerInfo info)
		{
			_browser.Refresh(info);
		}

		public void UI_RequestRefresh()
		{
			ServerInfo.MarkAllInvalid(10);
		    _browser.Refresh();
		}

		public IEnumerable<IServerInfo> JS_RequestServers(int min, int max)
		{
			if (_sortedServers == null)
			{
				_sortedServers = _readyServers.Values.Where(info => info.Valid).ToList();
				_sortedServers.Sort((info, serverInfo) => info.LastUpdate.CompareTo(serverInfo.LastUpdate));
			}

			min = Math.Max(min, 0);
			max = Math.Min(max, _sortedServers.Count);

			return _sortedServers.Skip(min).Take(max);
		}

		public ServerManager(Dispatcher mainDispatcher)
		{
			_browser = new ServerBrowser();
			_readyServers = new ConcurrentDictionary<string, ServerInfo>();
			_dispatcher = mainDispatcher;

			_browser.ServerChanged += BrowserOnServerChanged;

			_browser.Refresh(true);
		}

		private static Func<string, ServerInfo, ServerInfo> _makeUpdate(QueryMaster.ServerInfo newInfo)
		{
			return (s, info) =>
			{
				info.Update(newInfo);
				return info;
			};
		}

		private static Func<string, ServerInfo> _makeAdd(QueryMaster.ServerInfo newInfo)
		{
			return s => new ServerInfo(newInfo);
		}

		private void BrowserOnServerChanged(object sender, ServerEventArgs serverEventArgs)
		{
			_readyServers.AddOrUpdate(serverEventArgs.Handle, _makeAdd(serverEventArgs.Server),
				_makeUpdate(serverEventArgs.Server));
		}

		public IServerInfo Find(string jsHandle)
		{
			ServerInfo info;
			_readyServers.TryGetValue(jsHandle, out info);
			return info;
		}

		public int ServerCount
		{
			get { return _readyServers.Count; }
		}

		public void Refresh(string handle)
		{
			if(Thread.CurrentThread == _dispatcher.Thread)
				UI_RequestUpdate(handle);
			else JS_RequestUpdate(handle);
		}

		public void Refresh()
		{
			if (Thread.CurrentThread == _dispatcher.Thread)
				UI_RequestRefresh();
			else JS_RequestRefresh();
		}

		public IEnumerable<IServerInfo> ServerList
		{
			get { return _readyServers.Values; }
		}
	}
}
