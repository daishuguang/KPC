using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	public enum RouteAcquireAction
	{
		[Description("请求/被请求")]
		[EnumMember]
		Request = 0,

		[Description("确认")]
		[EnumMember]
		Confirm = 1,

		[Description("拒绝")]
		[EnumMember]
		Reject = -1,

		[Description("取消")]
		[EnumMember]
		Cancel = -2
	}
}
