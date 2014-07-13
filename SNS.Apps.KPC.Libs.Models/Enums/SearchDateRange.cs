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
	public enum SearchDateRange
	{
		[Description("工作日")]
		[EnumMember]
		Workday = 0x01,

		[Description("周末")]
		[EnumMember]
		Weekend = 0x02,

		[Description("临时")]
		[EnumMember]
		Temp = 0x04,

		[Description("全部")]
		[EnumMember]
		EveryDay = 0x08,

		[Description("上午")]
		[EnumMember]
		InMorning = 0x10,

		[Description("下午")]
		[EnumMember]
		InAfternoon = 0x20,

		[Description("下午")]
		[EnumMember]
		EntireDay = 0x40
	}
}
