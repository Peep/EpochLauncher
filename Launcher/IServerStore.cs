using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
	public interface IServerStore
	{
		IServerInfo Find(int jsHandle);
		int ServerCount { get; }
		void Refresh(int handle);
		void Refresh();
		IEnumerable<IServerInfo> ServerList { get; } 
	}

}
