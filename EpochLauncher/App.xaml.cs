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
			using (var manager = new UpdateManager(@"Releases\", "EpochLauncher", FrameworkVersion.Net45))
			{
				SquirrelAwareApp.HandleEvents(
					onInitialInstall: new Action<Version>(Application_OnInitalInstall),
					onAppUpdate: new Action<Version>(Application_OnUpdate),
					onAppObsoleted: new Action<Version>(Application_OnObsolete),
					onFirstRun: new Action(Application_OnFirstRun),
					onAppUninstall: new Action<Version>(Application_OnUninstall));
		
				await manager.UpdateApp();
			}
		}
	}
}
