using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ViewModels
{
	public struct ServerHandleViewModel
		: IEquatable<ServerViewModel>, IEquatable<int>, IEquatable<ServerHandleViewModel>
	{
		public readonly ServerViewModel Server;
		public readonly int JsHandle;

		public ServerHandleViewModel(ServerViewModel server)
		{
			Server = server;
			JsHandle = string.Format("{0}:{1}", Server.IpAddress, Server.Port).GetHashCode();
		}

		public ServerHandleViewModel(int jsHandle)
		{
			JsHandle = jsHandle;
			Server = null; //ServerStore.Find(jsHandle);
		}

		public bool Equals(ServerViewModel other)
		{
			return Server == other;
		}

		public bool Equals(int other)
		{
			return JsHandle == other;
		}

		public bool Equals(ServerHandleViewModel other)
		{
			var sa = JsHandle == other.JsHandle;
			var sb = Server == other.Server;

			if (sa ^ sb)
			{
				throw new Exception("AHHHH");
			}

			return sa && sb;
		}
	}
}
