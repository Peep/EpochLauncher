using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using System.IO;

namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public class ServerTable
        {
	        private class ServerData
	        {
		        public readonly uint Id;
				public string Ip;
		        public string Name;
		        public string MapName;
		        public float Ping;
		        public uint Port;
				public uint MinPlayers;
				public uint MaxPlayers;

		        internal static uint _nextId = 0;

				public ServerData(string ip, string name, uint port, uint minPlayers, uint maxPlayers)
				{
					Id = ++_nextId;
			        Ip = ip;
			        Name = name;
					Port = port;
			        MinPlayers = minPlayers;
					MaxPlayers = maxPlayers;
				}

	        }


            public class ServerTableProxy
            {
	            private readonly ServerTable _table;


	            public ServerTableProxy(ServerTable table)
	            {
		            _table = table;
	            }


	            public string RequestNeedUpdates(int count)
	            {
		            var result = new List<ServerData>();
		            var touched = new HashSet<uint>();

		            uint data;
		            while (count > 0 && _table._dirtyServers.TryDequeue(out data))
		            {
			            if (touched.Contains(data))
							continue;
	
				          touched.Add(data);
				          result.Add(_table._servers[data]);
			            
		            }

		            return JsonConvert.SerializeObject(result);
	            }
            }


            private readonly ServerTableProxy _proxy;
	        private readonly ConcurrentQueue<uint> _dirtyServers;
			private readonly Dictionary<uint, ServerData> _servers;
	        private readonly Task _poker;


            public ServerTable()
            {
	            _proxy = new ServerTableProxy(this);

	            var servers = new []
	            {
		            new ServerData("188.165.250.119", "Some? Server", 2364, 50, 50),
					new ServerData("188.165.233.104", "BMRF Server 1", 2502, 0, 50),
					new ServerData("188.165.250.119", "BMRF Server 2", 2602, 25, 50),
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
		    private MainWindow _window;

		    public delegate void LauncherWindowEvent();

		    public event LauncherWindowEvent CloseEvent;
		    public event LauncherWindowEvent MinimizeEvent;
		    public event LauncherWindowEvent MaximizeEvent;


		    public BoundMessager(MainWindow window)
		    {
			    _window = window;

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

        public MainWindow()
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
