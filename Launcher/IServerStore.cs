using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
	public interface IServerStore
	{
		IServerInfo Find(string jsHandle);


		int ServerCount { get; }
		IEnumerable<IServerInfo> ServerList { get; } 
	}

}
