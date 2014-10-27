using System;
using System.Net;
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
        public Version Version = new Version("0.1.1");

		private void Application_Startup(object sender, StartupEventArgs e)
		{
		    var uri = new Uri("https://launcher.bmrf.me/version.json");
		    var wc = new WebClient();
            wc.DownloadStringCompleted += async (s, args) => await Task.Run(() => CheckForUpdate(args.Result));
		    wc.DownloadStringAsync(uri);
		}

        private void CheckForUpdate(string json)
        {
            var latestVersion = JsonConvert.DeserializeObject<Version>(json);
            if (latestVersion > Version)
            {
                MessageBox.Show("Out of date!");
                // update the app   
            }
        }
	}
}
