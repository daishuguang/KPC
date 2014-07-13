using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	public enum RouteStatus
	{		
		[Description("有效")]
		Available = 0,

		[Description("失效")]
		Inavailable = -1
	}
}
