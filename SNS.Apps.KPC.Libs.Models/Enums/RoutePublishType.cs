using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	public enum RoutePublishType
	{
		[Description("由乘客发布")]
		[EnumMember]
		Publish_By_Passenger = 0,

		[Description("由司机发布")]
		[EnumMember]
		Publish_By_Driver = 1
	}
}
