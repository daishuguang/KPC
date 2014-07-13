using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public enum RouteDirection
	{
		[EnumMember]
		From = 0,

		[EnumMember]
		To = 1
	}
}
