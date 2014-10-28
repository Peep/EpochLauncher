using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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

        public MainWindow()
        {
            InitializeComponent();

			var settings = new CefSettings
			{
				PackLoadingDisabled = false,
				

			};

	        if (!Cef.Initialize(settings)) return;


	        WebView = new ChromiumWebBrowser
	        {
		        BrowserSettings = new BrowserSettings
		        {
			        FileAccessFromFileUrlsAllowed = true,
					WebGlDisabled = true,
					JavaDisabled = true,	
		        },

	        };

	        WebView.Loaded += delegate
	        {
				WebView.PreviewMouseDown += delegate
		        {
					DragMove();
				};
	        };

			Browser.Children.Add(WebView);
			WebView.Address = "cdn.bmrf.me/responsive.html";
	        Messager = new BoundMessager(this);
			Messager.CloseEvent += MessagerOnCloseEvent;
			Messager.MinimizeEvent += MessagerMinimizeEvent;
			Messager.MaximizeEvent += MessagerOnMaximizeEvent;
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
		    WebView.RegisterJsObject("launcher", Messager);
		    WebView.ShowDevTools();
	    }
    }
}
