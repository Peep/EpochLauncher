using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Launcher;
using QueryMaster;

namespace EpochLauncher.Tests
{
    [TestClass]
    public class BrowserTests
    {
        [TestMethod]
        public void CAN_QUERY_SERVERS_QUICKLY()
        {
            var servers = new List<ServerInfo>();
            var browser = new ServerBrowser();

            browser.ServerChanged += (s, e) => servers.Add(e.Server);
            browser.Refresh();

            Thread.Sleep(1500);
            Assert.IsTrue(servers.Count(s => s != null) > 8);
        }
    }
}
