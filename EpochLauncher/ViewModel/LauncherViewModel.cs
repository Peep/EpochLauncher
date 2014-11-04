using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
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

			public static readonly HashSet<string> OfficalIps = new HashSet<string>
			{
				"128.165.214.23",
				"5.62.103.29",
				"88.198.221.81",
				"69.162.70.162",
				"193.192.58.69",
				"192.254.71.234",
				"178.33.137.135",
				"192.95.30.52",
				"192.99.101.126",
				"188.165.250.119",
				"62.210.83.13",
				"188.165.233.104",
				"192.254.71.233",
				"103.13.103.37",
				"178.33.226.75",
				"69.162.93.250"
			};

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
			public AppSettings _weee;

			public IServerInfo QuickLaunch;

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
				if (QuickLaunch == null)
				{

					var a = File.Exists(_weee.gamePath);

					Process.Start(_weee.gamePath, string.Format("-mod=@Epoch"));
					return ResultSuccess;
				}

				return ManualConnectTo(QuickLaunch.Address, int.Parse(QuickLaunch.Port));
			}

		
			public string ConnectTo(int serverHash)
			{
				var server = _serverStore.Find(serverHash);
				if (server == null)
				{
					return @"{""result"":""unknownServer""}";
				}


				return ManualConnectTo(server.Address, int.Parse(server.Port));
			}

			public string ManualConnectTo(string domain, int port)
			{
				domain = Dns.GetHostAddresses(domain).First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();


				var connectArgs = string.Format("-mod=@Epoch -connect={0} -port={1}", domain, port);
				File.AppendAllText("connect.log", connectArgs + "\n");
				Process.Start(_weee.gamePath, connectArgs);
				return ResultSuccess;
			}

			public string GetQuickLaunch()
			{
				if (QuickLaunch != null)
				{		
					QuickLaunch = _serverStore.Find(QuickLaunch.Handle);
				}

				return QuickLaunch != null ? JsonConvert.SerializeObject(QuickLaunch) : @"{""Handle"":0}";
			}

			public void SetQuickLaunch(int jsHandle)
			{
				if (jsHandle == 0)
					QuickLaunch = null;
				else
					QuickLaunch = _serverStore.Find(jsHandle);
			}

			public string RequestServers(int min, int max)
			{
				IServerInfo[] serverList;
				lock (_serverStore.ServerList)
				{
					max = Math.Min(_serverStore.ServerCount, max);
					min = Math.Max(0, min);
					serverList =
						_serverStore.ServerList.Skip(min).Take(max - min).Where(ip => OfficalIps.Contains(ip.Address)).ToArray();
				}
				return JsonConvert.SerializeObject(serverList);
			}

			public string RequestServer(int jsHandle)
			{
				IServerInfo result;
				lock (_serverStore.ServerList)
				{
					_serverStore.Refresh(jsHandle);
					result = _serverStore.Find(jsHandle);
					if (result == null)
					{
						return null;
					}
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
				private string _addr;
				private string _port;

				public BOOTYSWEAT(ServerInfo info)
				{
					_underTheSea = info;
					var split = info.Address.Split(new [] {':'});
					if (split.Length != 2)
					{
						_addr = "0.0.0.0";
						_port = "0000";
					}
					else
					{
						_addr = split[0].Trim();
						_port = split[1].Trim();
					}
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
					get { return _underTheSea.Extra.Port.ToString(); }
				}

				public string Address
				{
					get { return _addr; }
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

			public void Refresh(int handle)
			{
				_bowbow.Refresh(handle);
			}

			public void Refresh()
			{
				_bowbow.Refresh();
			}

			public int ServerCount { get { return _bowbow.ServerCount; } }

			public IEnumerable<IServerInfo> ServerList
			{
				get { return _bowbow.Servers.Values.Select(x => new BOOTYSWEAT(x)); }
			}
		}





		private LauncherView _view;
		private readonly JSAdapter _jsAdapter;

		public readonly IServerStore ServerStore;
		public readonly IGameLauncher Launcher;
		public readonly AppSettings Settings;

		


		public LauncherViewModel(LauncherView view)
		{
			_view = view;
			ServerStore = new FiddlyDiddlyGottaHaveSomeBooty();
			_jsAdapter = new JSAdapter(null, (FiddlyDiddlyGottaHaveSomeBooty)ServerStore, _view);

			Settings = AppSettings.GetSettings("app.json");
			_jsAdapter._weee = Settings;
			_jsAdapter.QuickLaunch = Settings.quickLaunch;
		}

		public void Register(IWebBrowser browser)
		{
			browser.RegisterJsObject("launcher", _jsAdapter);
		}

		public void OnClosing()
		{
			Settings.quickLaunch = new SeralizedServerInfo(_jsAdapter.QuickLaunch);
		}

	}
}
