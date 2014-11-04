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
		void Refresh(string handle);
		void Refresh();
		IEnumerable<IServerInfo> ServerList { get; } 
	}

}
