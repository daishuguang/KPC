using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	public enum RouteRepeatCondition
	{
		[Description("不重复")]
		[EnumMember]
		Repeat_None = 0,

		[Description("周日")]
		[EnumMember]
		Repeat_Sunday = 1,

		[Description("周一")]
		[EnumMember]
		Repeat_Monday = 2,

		[Description("周二")]
		[EnumMember]
		Repeat_Tuesday = 4,

		[Description("周三")]
		[EnumMember]
		Repeat_Wednesday = 8,

		[Description("周四")]
		[EnumMember]
		Repeat_Thursday = 16,

		[Description("周五")]
		[EnumMember]
		Repeat_Friday = 32,

		[Description("周六")]
		[EnumMember]
		Repeat_Saturday = 64
	}
}
