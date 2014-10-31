using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
	    private readonly LauncherViewModel _viewModel;

	    public readonly ChromiumWebBrowser WebView;

        public LauncherView()
        {
            InitializeComponent();

			_viewModel = new LauncherViewModel(this);

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
                    AcceleratedCompositingDisabled = !_viewModel.Settings.enableComposting
                   
		        },

				Width = _viewModel.Settings.width,
				Height = _viewModel.Settings.height,
				Address = _viewModel.Settings.uri,
	        };

			Browser.Children.Add(WebView);
	        if (_viewModel.Settings.showTools)
	        {
		        WebView.ShowDevTools();
	        }
			_viewModel.Register(WebView);

        }

		private void LauncherView_Loaded(object sender, RoutedEventArgs e)
		{
 			
		}

	    private void LauncherView_OnClosing(object sender, CancelEventArgs e)
	    {
			_viewModel.OnClosing();
			_viewModel.Settings.Save("app.json");
	    }
    }
}
