using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherLib.ServerBrowser
{
    public class ServerBrowser
    {
        public string Test { get; private set; }

        public ServerBrowser()
        {
            Test = "hi";
        }

        public ServerBrowser(string source, ServerBrowserSourceType type)
        {
            if (type == ServerBrowserSourceType.Json)
            {

            }
        }

        public enum ServerBrowserSourceType
        {
            Json = 1,
            Text = 2,
            Xml = 3
        }
    }
}
