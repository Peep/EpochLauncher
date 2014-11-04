using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Launcher;
using Launcher.Events;

namespace Launcher.ConsoleTest
{
    class Program
    {
        private static ServerBrowser browser;
        static void Main()
        {
            browser = new ServerBrowser();
            browser.ServerAdded += OnServerAdded;
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

        static void OnServerAdded(object sender, ServerEventArgs e)
        {
            Console.WriteLine("{0} ({1} players)", e.Server.Name, e.Server.Players);
        }

        static void OnServerChanged(object sender, ServerEventArgs e)
        {


        }
    }
}
