using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Squirrel;



namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		static bool ShowWelcome = false;


		static void Application_OnInitalInstall(Version x)
		{
			Console.WriteLine(x.Build);
		}

		static void Application_OnUpdate(Version x)
		{
			Console.WriteLine(x.Build);
		}

		static void Application_OnUninstall(Version x)
		{
			Console.WriteLine(x.Build);
		}

		static void Application_OnFirstRun()
		{
			Console.WriteLine("First Run");
		}

		static void Application_OnObsolete(Version x)
		{
			Console.WriteLine(x.Build);
		}


		private async void Application_Startup(object sender, StartupEventArgs e)
		{



#if !DEBUG
#else
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
	}
}
