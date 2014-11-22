using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CefSharp.Wpf;
using Launcher;
using log4net;
using Newtonsoft.Json;

namespace EpochLauncher.Model
{
	public class ExportedJavascriptHandler
	{
		internal readonly ChromiumWebBrowser WebBrowser;
		internal readonly IJavascriptHandler Handler;
		internal readonly Dispatcher Dispatcher;
		internal readonly IServerStore Servers;


		public ExportedJavascriptHandler(IJavascriptHandler intHandler, ChromiumWebBrowser web)
		{
			Handler = intHandler;
			WebBrowser = web;
			Dispatcher = Handler.Dispatcher;
		}

		public void Minimize()
		{
			Dispatcher.InvokeAsync(Handler.MinimizeMessage);
		}

		public void Maximize()
		{
			Dispatcher.InvokeAsync(Handler.MaximizeMessage);
		}

		public void Close()
		{
			Dispatcher.InvokeAsync(Handler.CloseMessage);
		}

		public string GetQuickLaunch()
		{
			return JsonConvert.SerializeObject(Handler.GetQuickLaunchMessage());
		}

		public void SetQuickLaunch(string server)
		{
			var info = Servers.Find(server);
			Handler.SetQuickLaunchMessage(info);
		}

		public string GetConfiguration()
		{
			return JsonConvert.SerializeObject(Handler.GetConfigurationMessage());
		}

		public void SetConfiguration(string config)
		{
			Handler.SetConfigurationMessage(JsonConvert.DeserializeObject<LauncherOptions>(config));
		}

		public void OpenFileDialog(string reasonString)
		{
			FileDialogReason reason;
			if (Enum.TryParse(reasonString, out reason))
			{
				Dispatcher.InvokeAsync(() => Handler.OpenFileDialogMessage(reason));
			}
			else
			{
				LogManager.GetLogger("CEF").WarnFormat("OpenFileDialog : Invalid Argument : reasonString|{0} : Ignoring.", reasonString);
			}
		}

		public string StartGame()
		{
			return string.Format(@"{{ ""result"" : ""{0}"" }}",
				Dispatcher.Invoke(() => Handler.StartGame(Handler.GetQuickLaunchMessage())));
		}
	}
}
