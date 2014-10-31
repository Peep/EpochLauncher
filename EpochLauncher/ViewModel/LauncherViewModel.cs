﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

			private IServerInfo _quickLaunch;

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
				if (_quickLaunch == null)
				{
					Process.Start("arma3.exe", string.Format("-mod=@Epoch -nosplash"));
					return ResultSuccess;
				}

				return ManualConnectTo(_quickLaunch.Address, int.Parse(_quickLaunch.Port));
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

				Process.Start("arma3.exe", string.Format("-mod=@Epoch -nosplash -connect={0} -port={1}", domain, port));
				return ResultSuccess;
			}

			public string GetQuickLaunch()
			{
				return _quickLaunch != null ? JsonConvert.SerializeObject(_quickLaunch) : null;
			}

			public void SetQuickLaunch(int jsHandle)
			{
				if (jsHandle == 0)
					_quickLaunch = null;
				else
					_quickLaunch = _serverStore.Find(jsHandle);
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
					get { return _port; }
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
