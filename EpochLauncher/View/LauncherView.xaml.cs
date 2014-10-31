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

	    public readonly ChromiumWebBrowser WebView;

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
	        WebView.ShowDevTools();



        }

		private void LauncherView_Loaded(object sender, RoutedEventArgs e)
		{
 			
		}
    }
}
