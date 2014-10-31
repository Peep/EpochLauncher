using System;
using System.Collections.Generic;
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
            browser.ServerChanged += OnServerChanged;
            browser.Refresh();
            while (true) ;
        }

        static void OnServerAdded(object sender, ServerEventArgs e)
        {
            Console.WriteLine("{0} ({1}) players", browser.Servers[e.Handle].Name, browser.Servers[e.Handle].Players);
        }

        static void OnServerChanged(object sender, ServerEventArgs e)
        {

        }
    }
}
