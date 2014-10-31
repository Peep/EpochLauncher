using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using EpochLauncher.ViewModel;
using Newtonsoft.Json;
using System.IO;

namespace EpochLauncher.View
{
    /// <summary>
    /// Interaction logic for LauncherView.xaml
    /// </summary>
    public partial class LauncherView : Window
    {
	    private LauncherViewModel _viewModel;

	    public readonly ChromiumWebBrowser WebView;

        public LauncherView()
        {
            InitializeComponent();



			_viewModel = new LauncherViewModel(this);

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
			WebView.FrameLoadEnd += WebView_FrameLoadEnd;
            WebView.Address = "http://cdn.bmrf.me/UI.html"; //Jamie. Point me at the WebUI folder. 
	        WebView.ShowDevTools();
			_viewModel.Register(WebView);





        }

		void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			
		}

		private void LauncherView_Loaded(object sender, RoutedEventArgs e)
		{
 			
		}
    }
}
