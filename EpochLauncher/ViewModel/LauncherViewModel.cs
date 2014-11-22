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
using System.Windows.Controls;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using EpochLauncher.View;
using Launcher;
using Newtonsoft.Json;
using QueryMaster;

namespace EpochLauncher.ViewModel
{
	public class LauncherViewModel
	{

		public interface IJavascriptInterface
		{
			Dispatcher Dispatcher { get; }

			void Minimize();
			void Maximize();
			void Close();
			string StartGame();
			string ConnectTo(string ident);
			string ManualConnectTo(string domain, int port);
			string GetQuickLaunch();
			void SetQuickLaunch(string ident);
			string RequestServers(int min, int max);
			string RequestServer(string ident);
			void OpenSteamFileDialog();
		}


		public class ExportedJavascriptInterface
			: IJavascriptInterface
		{
			private readonly IJavascriptInterface _internalInterface;
			private readonly ChromiumWebBrowser _browser;


			public Dispatcher Dispatcher
			{
				get { return _browser.Dispatcher; }
			}

			public void Minimize()
			{
				_internalInterface.Dispatcher.Invoke(_internalInterface.Minimize);
			}

			public void Maximize()
			{
				_internalInterface.Dispatcher.Invoke(_internalInterface.Maximize);
			}

			public void Close()
			{
				_internalInterface.Dispatcher.Invoke(_internalInterface.Close);
			}

			public string StartGame()
			{
				return _internalInterface.Dispatcher.Invoke(new Func<string>(_internalInterface.StartGame));
			}

			public string BeginConnectTo(string ident)
			{
				return _internalInterface.Dispatcher.Invoke(() => _internalInterface.ConnectTo(ident));
			}

			public void EndConnectTo()
			{
				_browser.ExecuteScriptAsync("");
			}

			public string ManualConnectTo(string domain, int port)
			{
				return _internalInterface.Dispatcher.Invoke(() => _internalInterface.ManualConnectTo(domain, port));
			}

			public string GetQuickLaunch()
			{
				throw new NotImplementedException();
			}

			public void SetQuickLaunch(string ident)
			{
				throw new NotImplementedException();
			}

			public string RequestServers(int min, int max)
			{
				throw new NotImplementedException();
			}

			public string RequestServer(string ident)
			{
				throw new NotImplementedException();
			}

			public void OpenSteamFileDialog()
			{
				throw new NotImplementedException();
			}
		}


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

			public JSAdapter(IGameLauncher launcher, ServerManager serverStore, LauncherView view)
			{
				_view = view;
				_launcher = launcher;
				_serverStore = serverStore;
			}

			
			private List<IServerInfo> _sortedServers;
			private LauncherView _view;
			private IGameLauncher _launcher;
			private ServerManager _serverStore;
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

		
			public string ConnectTo(string serverHash)
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

				return QuickLaunch != null ? JsonConvert.SerializeObject(QuickLaunch) : @"{""Handle"":""""}";
			}

			public void SetQuickLaunch(string jsHandle)
			{
				if (jsHandle == null)
					QuickLaunch = null;
				else
					QuickLaunch = _serverStore.Find(jsHandle);
			}

			public string RequestServers(int min, int max)
			{
				_serverStore.JS_RequestRefresh();
				return JsonConvert.SerializeObject(_serverStore.JS_RequestServers(min, max));
			}

			public string RequestServer(string jsHandle)
			{
				return JsonConvert.SerializeObject(_serverStore.JS_RequestServer(jsHandle));
			}

			public void OpenSteamFileDialog()
			{
				_view.Dispatcher.InvokeAsync(_view.)
			}
		}

		private LauncherView _view;
		private readonly JSAdapter _jsAdapter;

		public readonly ServerManager ServerStore;
		public readonly IGameLauncher Launcher;
		public readonly AppSettings Settings;

		


		public LauncherViewModel(LauncherView view)
		{
			_view = view;
			ServerStore = new ServerManager(view.Dispatcher);
			_jsAdapter = new JSAdapter(null, ServerStore, _view);

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
