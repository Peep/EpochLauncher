using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

			private List<IServerInfo> _sortedServers;  

			public LauncherView View { get; set; }
			public IGameLauncher Launcher { get; set; }
			public IServerStore ServerStore { get; set; }


			public void Minimize()
			{
				View.Dispatcher.InvokeAsync(() => View.WindowState = WindowState.Minimized);
			}

			public void Maximize()
			{
				View.Dispatcher.InvokeAsync(() => View.WindowState = WindowState.Maximized);
			}

			public void Close()
			{
				View.Dispatcher.InvokeAsync(View.Close);
			}

			public string Start()
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
				var result = ServerStore.Find(jsHandle);
				if (result == null)
				{
					return JsonConvert.SerializeObject(null);
				}

				return JsonConvert.SerializeObject(result);
			}
		}

		private LauncherView _view;
		private JSAdapter _jsAdapter;


		public LauncherViewModel(LauncherView view)
		{
			_view = view;
			_jsAdapter = new JSAdapter
			{
				Launcher = null,
				ServerStore = null,
				View = _view
			};

			_view.Loaded += view_Loaded;

			

		}

		void view_Loaded(object sender, RoutedEventArgs e)
		{
			_view.WebView.RegisterJsObject("launcher", _jsAdapter);
		}

	}
}
