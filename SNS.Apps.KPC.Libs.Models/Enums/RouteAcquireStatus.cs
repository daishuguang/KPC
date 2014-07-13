using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	public enum RouteAcquireStatus
	{
		[Description("未关联")]
		[EnumMember]
		NoSet = 0,

		[Description("等待确认")]
		[EnumMember]
		Pending = 1,

		[Description("已确认")]
		[EnumMember]
		Confirmed = 2,

		[Description("已确认 - 乘客")]
		[EnumMember]
		ConfirmedWithFellow,

		[Description("已拒绝")]
		[EnumMember]
		Rejected = -1
	}
}
