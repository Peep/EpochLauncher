using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
	public class LaunchResult
	{
		public enum ResultType
		{
			Success,
			Failure,
			ConnectionError,
			ServerError,
		}

		public ResultType Result { get; set; }
		public string Info { get; set; }
	}


	public interface IGameLauncher
	{
		string GameName { get; }
		string GamePath { get; }

		void OpenDirectory();
		LaunchResult Launch(params string[] args);

	}
}
