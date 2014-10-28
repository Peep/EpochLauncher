using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


using CefSharp;

namespace EpochLauncher
{
    public class LocalFileResourceHandler
        : IRequestHandler
    {
        public static readonly Uri UriPrefix = new Uri("file:///");
        public static readonly Encoding Encoder = UnicodeEncoding.Unicode;
        public static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            {"png", "image/png"},
            {"html", "text/html"},
        };

        public bool LoadLocalResource(IRequestResponse request)
        {
            var localUri = new Uri(request.Request.Url.ToString());

            if (!localUri.IsFile)
                return false;

            var bytes = File.ReadAllBytes(localUri.AbsolutePath);
            request.RespondWith(new MemoryStream(bytes, false), "text/html");
            return true;
        }




        public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password)
        {
            return false;
        }

        public bool OnBeforeBrowse(IWebBrowser browser, IRequest request, bool isRedirect)
        {
            return false;
        }


        public bool OnBeforePluginLoad(IWebBrowser browser, string url, string policyUrl, IWebPluginInfo info)
        {
            return false;
        }

        public bool OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {
            var req = requestResponse.Request;

            if (!req.Url.StartsWith(UriPrefix.ToString()))
                return false;

            var nativeUri = new Uri(req.Url.ToString());
            var filepath = nativeUri.AbsolutePath;

           

            var stream = new MemoryStream(File.ReadAllBytes(filepath), false);
            requestResponse.RespondWith(stream, "text/html");


            return true;
        }

     

        public void OnPluginCrashed(IWebBrowser browser, string pluginPath)
        {

        }

        public void OnRenderProcessTerminated(IWebBrowser browser, CefTerminationStatus status)
        {

        }
    }
}
