using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();

			var settings = new CefSettings {PackLoadingDisabled = true};

	        if (!Cef.Initialize(settings)) return;


	        var webview = new ChromiumWebBrowser();
	        Browser.Children.Add(webview);
	        webview.Address = "http://www.google.com";
        }

	    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	    {
		 
	    }
    }
}
