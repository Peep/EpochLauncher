using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using EpochLauncher.View;
using Launcher;
using Newtonsoft.Json;
using QueryMaster;

namespace EpochLauncher.ViewModel
{
	public class LauncherViewModel
	{
		private class JSAdapter
		{
			private const string ResultSuccess = @"{""result"":""success""}";

			public JSAdapter(IGameLauncher launcher, FiddlyDiddlyGottaHaveSomeBooty serverStore, LauncherView view)
			{
				_view = view;
				_launcher = launcher;
				_serverStore = serverStore;
			}

			private List<IServerInfo> _sortedServers;
			private LauncherView _view;
			private IGameLauncher _launcher;
			private FiddlyDiddlyGottaHaveSomeBooty _serverStore;

			public void Minimize()
			{
				_view.Dispatcher.InvokeAsync(() => _view.WindowState = WindowState.Minimized);
			}

			public void Maximize()
			{
				_view.Dispatcher.InvokeAsync(() => _view.WindowState = WindowState.Maximized);
			}

			public void Close()
			{
				_view.Dispatcher.InvokeAsync(_view.Close);
			}

			public string StartGame()
			{
				return ResultSuccess;
			}

			public string ConnectTo(int serverHash)
			{
				return ResultSuccess;
			}

			public string ManualConnectTo(string domain, int port)
			{
				return ResultSuccess;
			}

			public string RequestServers(int min, int max)
			{
				max = Math.Min(_serverStore.ServerList.Count(), max);
				min = Math.Max(0, min);
				var serverList = _serverStore.ServerList.Skip(min).Take(max - min).ToArray();
				return JsonConvert.SerializeObject(serverList);


				
				var list = new List<IServerInfo>(max - min);
				for (var i = min; i < max; i++)
				{
					//list[i - min] = serverList[i];
				}
				return JsonConvert.SerializeObject(list);
			}

			public string RequestServer(int jsHandle)
			{
				var result = _serverStore.Find(jsHandle);
				if (result == null)
				{
					return null;
				}

				return JsonConvert.SerializeObject(result);
			}
		}

		private class FiddlyDiddlyGottaHaveSomeBooty
			: IServerStore
		{
			public class BOOTYSWEAT
				: IServerInfo
			{
				private ServerInfo _underTheSea;

				public BOOTYSWEAT(ServerInfo info)
				{
					_underTheSea = info;
				}


				public string Name
				{
					get { return _underTheSea.Name; }
				}

				public int CurrentPlayers
				{
					get { return _underTheSea.Players; }
				}

				public int MaxPlayers
				{
					get { return _underTheSea.MaxPlayers; }
				}

				public string Port
				{
					get { return "PORT"; }
				}

				public string Address
				{
					get { return _underTheSea.Address; }
				}

				public string Map
				{
					get { return _underTheSea.Map; }
				}

				public int Ping
				{
					get { return (int)_underTheSea.Ping; }
				}

				public int Handle { get { return _underTheSea.Address.GetHashCode(); }}
			}

			private ServerBrowser _bowbow;

			public FiddlyDiddlyGottaHaveSomeBooty()
			{
				_bowbow = new ServerBrowser();
				_bowbow.Refresh();
			}

			public IServerInfo Find(int jsHandle)
			{
				ServerInfo data;
				if (_bowbow.Servers.TryGetValue(jsHandle, out data))
				{
					return new BOOTYSWEAT(data);
				}
				return null;
			}

			public IEnumerable<IServerInfo> ServerList
			{
				get { return _bowbow.Servers.Values.Select(x => new BOOTYSWEAT(x)); }
			}
		}



		private LauncherView _view;
		private readonly JSAdapter _jsAdapter;
		private readonly IServerStore _serverStore;
		private readonly IGameLauncher _launcher;

		


		public LauncherViewModel(LauncherView view)
		{
			_view = view;
			_serverStore = new FiddlyDiddlyGottaHaveSomeBooty();
			_jsAdapter = new JSAdapter(null, (FiddlyDiddlyGottaHaveSomeBooty)_serverStore, _view);
		}

		public void Register(IWebBrowser browser)
		{
			browser.RegisterJsObject("launcher", _jsAdapter);
		}

	}
}
