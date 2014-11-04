using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class DownloadManager
    {
       
    }

    public enum DownloadState
    {
        Downloading,
        Paused,
        Completed,
        Corrupted,
        UpdateAvailable
    }
}
