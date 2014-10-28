using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Version Version = new Version("0.1.1");

		private void Application_Startup(object sender, StartupEventArgs e)
		{
            Updater.CheckForUpdates();
		}
	}
}
