using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher;

namespace EpochLauncher
{
	public class ServerInfo
		: IServerInfo
	{

		internal static ConcurrentBag<ServerInfo> AllInstances = new ConcurrentBag<ServerInfo>();

		internal static void MarkAllInvalid(double seconds = -1)
		{
			lock (AllInstances)
			{
				foreach (var info in AllInstances)
					if (info.LastUpdate.AddSeconds(seconds) < DateTime.Now)
						info.Valid = false;
			}
		}

		internal DateTime LastUpdate;
		internal bool Valid;

		internal bool UpdatedRecently
		{
			get { return DateTime.Now < LastUpdate.AddSeconds(20.0); }
		}

		public string Name { get; private set; }
		public int CurrentPlayers { get; private set; }
		public int MaxPlayers { get; private set; }
		public string Port { get; private set; }
		public string Address { get; private set; }
		public string Handle { get; private set; }
		public int Ping { get; private set; }
		public string Map { get; private set; }

		public void Update(QueryMaster.ServerInfo info)
		{
			Name = info.Name;
			CurrentPlayers = info.Players;
			MaxPlayers = info.MaxPlayers;
			Port = info.Extra.Port.ToString();
			Address = info.Address.Split(':')[0];
			Handle = string.Format("{0}:{1}", Address, Port);
			Ping = (int)info.Ping;
			Map = info.Map;

			LastUpdate = DateTime.Now;
			Valid = true;


		}

		public ServerInfo(QueryMaster.ServerInfo info)
		{
			Update(info);

			AllInstances.Add(this);
		}


	}
}
