using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Launcher.Events;
using Timer = System.Timers.Timer;

namespace Launcher.ConsoleTest
{
    class Program
    {
        private static ServerBrowser browser;
        static void Main()
        {
            //TestBrowser();
            TestDownload();
            while (true) ;
        }

        static void TestDownload()
        {
            var manager = new DownloadManager(@"D:\Games\SteamLibrary\SteamApps\common\Arma 3");
            manager.Start();

            var progressTimer = new Timer();
            progressTimer.Elapsed += (s, e) => Console.WriteLine(manager.Manager.Progress);
            progressTimer.Interval = 500;
            progressTimer.Start();

            var speedTimer = new Timer();
            speedTimer.Elapsed += (s, e) => Console.WriteLine(manager.Manager.Engine.TotalDownloadSpeed);
            speedTimer.Interval = 500;
            speedTimer.Start();
        }

        static void TestBrowser()
        {
            browser = new ServerBrowser();
            browser.ServerChanged += OnServerChanged;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            browser.Refresh();
            while (true)
            {
                Thread.Sleep(10);
                if (browser.ServerCount > 4000)
                {
                    using (var sw = new StreamWriter("result.txt", true))
                    {
                        stopwatch.Stop();
                        sw.WriteLine("Queried {0} servers in {1} seconds.", browser.ServerCount,
                            stopwatch.Elapsed.TotalSeconds);
                    }
                    break;
                }
            }
        }

        static void OnServerChanged(object sender, ServerEventArgs e)
        {
            Console.WriteLine("{0} ({1} players)", e.Server.Name, e.Server.Players);
        }
    }
}
