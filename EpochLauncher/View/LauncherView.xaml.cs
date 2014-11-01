using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
	    private Point _lastPos;
	    private Point _clickPos;

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

			PreviewMouseLeftButtonDown += LauncherView_PreviewMouseLeftButtonDown;
			PreviewMouseLeftButtonUp += LauncherView_PreviewMouseLeftButtonUp;
	        PreviewMouseMove += LauncherView_PreviewMouseMove;

			Content = WebView;
	        if (_viewModel.Settings.showTools)
	        {
		        WebView.ShowDevTools();
	        }
			_viewModel.Register(WebView);

        }



	    private void LauncherView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	    {
		    
	    }

		private void LauncherView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_clickPos = e.GetPosition(this);
		}

	    private void LauncherView_PreviewMouseMove(object sender, MouseEventArgs e)
	    {
		    var current = e.GetPosition(this);
		    var fromClick = current - _clickPos;
		    if (e.LeftButton == MouseButtonState.Pressed)
		    {
			    if (fromClick.Length > 10.0)
			    {
				    DragMove();
			    }

		    }
		    _lastPos = current;
	    }


	    private void LauncherView_OnClosing(object sender, CancelEventArgs e)
	    {
			_viewModel.OnClosing();
			_viewModel.Settings.Save("app.json");
	    }
    }
}
