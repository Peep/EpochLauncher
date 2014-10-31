using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpochLauncher
{
	public class StartGameResult
	{
		public enum ResultType
		{
			Success,
			Failure,
			VersionOutOfDate,
			ConnectionError,
			UnknownServer,
			pLeAse_uDdAtE_ThE_GaMe_BeCasE_PeEp_Is_GaY_AnD_He_HaTeS_YoU_DOT_JpEg
		}

		public ResultType Result;
		public string Info;
	}
}
