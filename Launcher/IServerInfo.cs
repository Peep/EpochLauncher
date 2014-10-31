using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
	public interface IServerInfo
	{
		string Name { get; }
		int CurrentPlayers { get; }
		int MaxPlayers { get; }
		string Port { get; }
		string Address { get; }
		int Handle { get; }
		int Ping { get; }
		string Map { get; }
	}
}
