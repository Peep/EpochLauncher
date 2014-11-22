using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Launcher;

namespace EpochLauncher.Model
{
	public enum GameStartResult
	{
		Success,
		Failure,
		NotFound,
	};

	public enum FileDialogReason
	{
		ArmaPath,
	}

	public interface IJavascriptHandler
	{
		Dispatcher Dispatcher { get; }

		void MinimizeMessage();
		void MaximizeMessage();
		void CloseMessage();

		IServerInfo GetQuickLaunchMessage();
		void SetQuickLaunchMessage(IServerInfo info);

		LauncherOptions GetConfigurationMessage();
		void SetConfigurationMessage(LauncherOptions config);

		void OpenFileDialogMessage(FileDialogReason reason);

		GameStartResult StartGame(IServerInfo info);
	}
}
