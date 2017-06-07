using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synapse.Handlers.WinUtil
{
	public class WmiTimeout
	{
		public WmiTimeout(int appPoolAction, int winServiceAction, int winServiceRequery)
		{
			AppPoolAction = appPoolAction;
			WinServiceAction = winServiceAction;
			WinServiceRequery = winServiceRequery;
		}

		public int AppPoolAction { get; private set; }
		public int WinServiceAction { get; private set; }
		public int WinServiceRequery { get; private set; }
	}
}