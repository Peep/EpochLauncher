using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpochLauncher.Model
{
	public class LauncherOptions
	{
		public string ArmaPath { get; internal set; }
		public readonly DateTime LastUpdated = DateTime.Now;

		public LauncherOptions()
		{

		}

		public LauncherOptions(LauncherOptions left, LauncherOptions right)
		{
			if (left.LastUpdated < right.LastUpdated)
			{
				var t = left;
				left = right;
				right = t;
			}

			ArmaPath = left.ArmaPath ?? right.ArmaPath;
		}
	}
}
