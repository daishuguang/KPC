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
	public enum RouteAcquireType
	{
		[EnumMember]
		NoSet = 0,

		[Description("主动订阅（乘客与司机）")]
		[EnumMember]
		AcquireFor = 1,

		[Description("被动订阅（乘客与司机）")]
		[EnumMember]
		AcquireTo = 2,

		//[Description("主动订阅（乘客之间）")]
		//AcquireForFellow = 8,

		//[Description("被动订阅（乘客之间）")]
		//AcquireToFellow = 16
	}
}
