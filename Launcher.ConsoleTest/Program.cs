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
            Console.WriteLine(browser.ServerCount);
        }

        static void OnServerChanged(object sender, ServerEventArgs e)
        {


        }
    }
}
