using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using System.IO;

namespace EpochLauncher.View
{
    /// <summary>
    /// Interaction logic for LauncherView.xaml
    /// </summary>
    public partial class LauncherView : Window
    {
        public class ServerTable
        {
            public class ServerTableProxy
            {
	            private readonly ServerTable _table;

	            public ServerTableProxy(ServerTable table)
	            {
		            _table = table;
	            }


	            public string RequestServers(int page)
	            {
		            var result = new List<ServerData>();
		            var touched = new HashSet<uint>();

		            uint data;
		            while (_table._dirtyServers.TryDequeue(out data))
		            {
			            if (touched.Contains(data))
							continue;
	
				          touched.Add(data);
				          result.Add(_table._servers[data]);
			            
		            }

		            return JsonConvert.SerializeObject(result);
	            }

	            public string RequestFavorites(int page)
	            {
		            return JsonConvert.SerializeObject(_table._servers[2]);
	            }

	            public string RequestHistory(int page)
	            {
		            return JsonConvert.SerializeObject(_table._servers[1]);
	            }

	            public void SetQuickLaunch(int serverId)
	            {
		            
	            }

	            public string RequestQuickLaunch()
	            {
		            return JsonConvert.SerializeObject(_table._servers[3]);
	            }
            }


            internal readonly ServerTableProxy _proxy;
			internal readonly ConcurrentQueue<uint> _dirtyServers;
			internal readonly Dictionary<uint, ServerData> _servers;
			internal readonly Task _poker;


            public ServerTable()
            {
	            _proxy = new ServerTableProxy(this);

	            var servers = new []
	            {
		            new ServerData("188.165.250.119", "Some? Server", "A Really Shitty Map", 432.5f, 2364, 50, 50),
					new ServerData("188.165.233.104", "BMRF Server 1", "A Really Great Map", 32.0f, 2502, 0, 50),
					new ServerData("188.165.250.119", "BMRF Server 2", "Crytek Sponza", 80.5f, 2602, 25, 50),
	            };

	            _servers = new Dictionary<uint, ServerData>();
				_dirtyServers = new ConcurrentQueue<uint>();
	            foreach (var s in servers)
	            {
		            _servers[s.Id] = s;
					_dirtyServers.Enqueue(s.Id);
	            }

	            _poker = new Task(() =>
	            {
		            while (!_poker.IsCanceled)
		            {
			            _dirtyServers.Enqueue((uint) new Random().Next(1, (int) ServerData._nextId + 1));
						Thread.Sleep(500);
		            }
	            });

				_poker.Start();
            }


            public void Register(IWebBrowser browser)
            {
                browser.RegisterJsObject("servers", _proxy);
            }
        }
        



	    public class BoundMessager
	    {
		    private LauncherView _window;

		    public class StartGameEventArgs : EventArgs
		    {
			    public readonly bool ConnectTo;
			    public readonly string ServerIP;
			    public readonly uint ServerPort;

			    public StartGameEventArgs(string serverIp, uint serverPort)
			    {
				    if (serverIp == "" && serverPort == 0)
					    ConnectTo = false;
				    else
				    {
					    ServerIP = serverIp;
					    ServerPort = serverPort;
				    }
			    }
		    }

		    public delegate void LauncherWindowEvent();

		    public delegate void LauncherStartGameEvent(BoundMessager sender, StartGameEventArgs args);


		    public event LauncherStartGameEvent StartGameEvent;
		    public event LauncherWindowEvent CloseEvent;
		    public event LauncherWindowEvent MinimizeEvent;
		    public event LauncherWindowEvent MaximizeEvent;


		    public BoundMessager(LauncherView window)
		    {
			    _window = window;

		    }


		    public string StartGame(int id)
		    {
			    var tmp = StartGameEvent;
			    if (tmp != null)
			    {
				    var serverIP = "";
				    uint port = 0;

				    if (id != 0)
				    {
					    if (!_window.Servers._servers.ContainsKey((uint) id))
					    {
							return @"{""result"":""unknown server""}";
					    }

					    serverIP = _window.Servers._servers[(uint)id].Ip;
					    port = _window.Servers._servers[(uint) id].Port;
				    }
				    tmp(this, new StartGameEventArgs(serverIP, port));
			    }

			    return @"{""result"":""success""}";
		    }

		    public void HandleMessage(string type, string json)
		    {
			    MessageBox.Show(type, json);
		    }

		    public void Minimize()
		    {
				var temp = MinimizeEvent;
			    if (temp != null)
			    {
					new Task(() => temp()).Start();
			    }
		    }

			public void Close()
			{
				var temp = CloseEvent;
				if (temp != null)
				{
					new Task(() => temp()).Start();
				}
			}

		    public void Maximize()
		    {
			    var temp = MaximizeEvent;
			    if (temp != null)
			    {
					new Task(() => temp()).Start();
			    }
		    }
	    }

	    public readonly ChromiumWebBrowser WebView;
        public readonly BoundMessager Messager;
        public readonly ServerTable Servers;

        public LauncherView()
        {
            InitializeComponent();

            WindowSettings jsonSettings;
            if(File.Exists("window.json"))
            {
                jsonSettings = JsonConvert.DeserializeObject<WindowSettings>(File.ReadAllText("window.json"));
            }
            else
            {
                jsonSettings = new WindowSettings
                {
                    width = 1148,
                    height = 608,
                    enableComposting = false,
                    uri = "http://cdn.bmrf.me/UI.html"
                };
            }

			var settings = new CefSettings
			{
				PackLoadingDisabled = false
			};

	        if (!Cef.Initialize(settings)) return;


	        WebView = new ChromiumWebBrowser
	        {
		        BrowserSettings = new BrowserSettings
		        {
			        FileAccessFromFileUrlsAllowed = true,
					WebGlDisabled = true,
					JavaDisabled = true,	
                    UniversalAccessFromFileUrlsAllowed = true,
                    WebSecurityDisabled = false,
                    PluginsDisabled = false,
                    AcceleratedCompositingDisabled = !jsonSettings.enableComposting,
                   
		        },

	        };

			Browser.Children.Add(WebView);

            WebView.Address = "http://cdn.bmrf.me/UI.html"; //Jamie. Point me at the WebUI folder. 
            Messager = new BoundMessager(this);
			Messager.CloseEvent += MessagerOnCloseEvent;
			Messager.MinimizeEvent += MessagerMinimizeEvent;
			Messager.MaximizeEvent += MessagerOnMaximizeEvent;
            WebView.RegisterJsObject("launcher", Messager);
            Servers = new ServerTable();
            Servers.Register(WebView);

            WebView.ShowDevTools();

        }

	    private void MessagerOnMaximizeEvent()
	    {
			Dispatcher.Invoke(() =>
			{
				WindowState = WindowState.Maximized;
			});
	    }

	    private void MessagerOnCloseEvent()
	    {
		    Dispatcher.Invoke(Close);
	    }

	    private void MessagerMinimizeEvent()
	    {
		    Dispatcher.Invoke(() =>
		    {
			    WindowState = WindowState.Minimized;
		    });
	    }

	    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
	    }
    }
}
