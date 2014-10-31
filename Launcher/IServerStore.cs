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

		IEnumerable<IServerInfo> ServerList { get; } 
	}


	public class ServerStore
		: IServerStore
	{
		private Dictionary<int, IServerInfo> _infos = new Dictionary<int, IServerInfo>();

		public void UpdateServer(object updateInfo)
		{
			
		}

		public void AddServer(object data)
		{
			
		}

		public IServerInfo Find(int jsHandle)
		{
			return _infos[jsHandle];
		}

		public IEnumerable<IServerInfo> ServerList
		{
			get { return _infos.Values; }
		}
	}

}
