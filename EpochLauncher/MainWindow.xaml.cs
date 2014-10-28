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

	    public class BoundMessager
	    {
		    public void HandleMessage(string type, string json)
		    {
			    MessageBox.Show(type, json);
		    }
	    }

	    public readonly ChromiumWebBrowser WebView;
	    public readonly BoundMessager Messager;

        public MainWindow()
        {
            InitializeComponent();

			var settings = new CefSettings {PackLoadingDisabled = true};

	        if (!Cef.Initialize(settings)) return;


			WebView = new ChromiumWebBrowser();
			Browser.Children.Add(WebView);
			WebView.Address = "test.html";
			Messager = new BoundMessager();
        }

	    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	    {
			WebView.RegisterJsObject("message", Messager);
		 
	    }
    }
}
