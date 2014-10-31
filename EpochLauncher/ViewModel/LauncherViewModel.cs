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

			public JSAdapter(IGameLauncher launcher, IServerStore serverStore, LauncherView view)
			{
				_view = view;
				_launcher = launcher;
				_serverStore = serverStore;
			}

			private List<IServerInfo> _sortedServers;
			private LauncherView _view;
			private IGameLauncher _launcher;
			private IServerStore _serverStore;

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
				max = Math.Min(_sortedServers.Count, max);
				min = Math.Max(0, min);

				var list = new List<IServerInfo>(max - min);
				for (var i = min; i < max; i++)
				{
					list[i - min] = _sortedServers[i];
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

		private LauncherView _view;
		private readonly JSAdapter _jsAdapter;


		public LauncherViewModel(LauncherView view)
		{
			_view = view;
			_jsAdapter = new JSAdapter(null, null, _view);
		}

		public void Register(IWebBrowser browser)
		{
			browser.RegisterJsObject("launcher", _jsAdapter);
		}

	}
}
