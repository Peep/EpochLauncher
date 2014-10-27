using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squirrel;



namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		static bool ShowWelcome = false;

        public Version Version = new Version("0.1.1");

		private async void Application_Startup(object sender, StartupEventArgs e)
		{
		    var uri = new Uri("https://launcher.bmrf.me/version.json");
		    var wc = new WebClient();
            wc.DownloadStringCompleted += async (s, args) => await Task.Run(() => CheckForUpdate(args.Result));
		    wc.DownloadStringAsync(uri);
#if !DEBUG
			using (var manager = new UpdateManager(@"http://dev.bmrf.me/launcher/", "EpochLauncher", FrameworkVersion.Net45))
			{
				var info = await manager.CheckForUpdate();
				if (info.FutureReleaseEntry == null)
				{
					MessageBox.Show(info.CurrentlyInstalledVersion.EntryAsString);
					return;
				}

				await manager.DownloadReleases(info.ReleasesToApply);
				await manager.ApplyReleases(info);
			}
#endif
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
