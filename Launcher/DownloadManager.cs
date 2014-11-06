using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MonoTorrent.Client;
using MonoTorrent.Common;

namespace Launcher
{
    public class DownloadManager
    {
        public ClientEngine Engine;
        public TorrentManager Manager;
        private readonly string _downloadPath;
        private XDocument _mirrorXml;

        public DownloadManager(string downloadPath)
        {
            Engine = new ClientEngine(new EngineSettings());
            _downloadPath = downloadPath;         
        }

        public void Start()
        {
            var torrent = Torrent.Load(DownloadTorrent("https://launcher.bmrf.me/Epoch-0.2.0.1.torrent"));
            Manager = new TorrentManager(torrent, _downloadPath, new TorrentSettings());
            Engine.Register(Manager);

            Manager.Start();
        }

        public DownloadState GetDownloadState()
        {
            if (!Directory.Exists(Path.Combine(_downloadPath, "@Epoch")))
                return DownloadState.Uninstalled;

            return DownloadState.Corrupted;
        }

        private byte[] DownloadTorrent(string uri)
        {
            using (var wc = new WebClient())
                return wc.DownloadData(uri);
        }
    }

    public enum DownloadState
    {
        Downloading,
        Paused,
        Completed,
        Uninstalled,
        Corrupted,
        UpdateAvailable
    }
}
