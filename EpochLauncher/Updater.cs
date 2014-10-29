using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace EpochLauncher
{
    public class Updater
    {
        public static void CheckForUpdates()
        {
            var uri = new Uri("https://launcher.bmrf.me/version.json");
            var wc = new WebClient();
            wc.DownloadStringCompleted += async (s, args) =>
                    await Task.Run(() => OnDownloadCompleted(args.Result));
            wc.DownloadStringAsync(uri);
        }

        private static void OnDownloadCompleted(string json)
        {
            var latestVersion = JsonConvert.DeserializeObject<Version>(json);
            if (false && latestVersion < App.Version)
                ShowUpdatePrompt();
        }

        private static void ShowUpdatePrompt()
        {
            var thread = new Thread(t =>
            {
                var window = new UpdatePromptWindow();
                window.Show();
                window.Closed += (s, e) => Dispatcher.ExitAllFrames();
                Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void DoUpdate()
        {
            Process.Start("notepad.exe");
            Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
        }
    }
}
