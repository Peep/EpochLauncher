using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher;
using Microsoft.Win32;
using Newtonsoft.Json;


namespace EpochLauncher
{
	public class SeralizedServerInfo
	: IServerInfo
	{
		public string Name { get; set; }
		public int CurrentPlayers { get; set; }
		public int MaxPlayers { get; set; }
		public string Port { get; set; }
		public string Address { get; set; }
		public int Handle { get; set; }
		public int Ping { get; set; }
		public string Map { get; set; }

		public SeralizedServerInfo()
		{
			Name = "";
			CurrentPlayers = 0;
			MaxPlayers = 0;
			Port = "00";
			Address = "0.0.0.0";
			Handle = 0;
			Ping = 0;
			Map = "";
		}

		public SeralizedServerInfo(IServerInfo copy)
		{
			if (copy == null)
				return;

			Name = copy.Name;
			CurrentPlayers = copy.CurrentPlayers;
			MaxPlayers = copy.MaxPlayers;
			Port = copy.Port;
			Address = copy.Address;
			Handle = copy.Handle;
			Ping = copy.Ping;
			Map = copy.Map;
		}
	}

    public class AppSettings
    {
        public uint width;
        public uint height;
        public bool enableComposting;
	    public bool showTools;
        public string uri;
	    public string gamePath;

		public SeralizedServerInfo quickLaunch;


	    public void Save(string path)
	    {
		    File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
	    }

		public static AppSettings GetSettings(string path)
	    {
		    if (File.Exists(path))
		    {
				return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path));
		    }

			var settings = new AppSettings
			{
				width = 1148,
				height = 596,
				enableComposting = false,
				showTools = true,
				uri = "http://cdn.bmrf.me/UI.html",
				quickLaunch = null
			};


			var unknown = new object();
			var steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\VALVE\STEAM", "SteamPath", unknown);
			if (steamPath == unknown)
			{
				throw new Exception("Steam not found, please manually set the gamePath property in app.json");
			}

			settings.gamePath = Path.GetFullPath(Path.Combine((string)steamPath, "steamapps", "common", "arma 3", "arma3.exe"));

		    File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
			return settings;
	    }
    }
}
