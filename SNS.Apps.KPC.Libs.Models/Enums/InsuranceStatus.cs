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
	public enum InsuranceStatus
	{
		[Description("等待审核")]
		[EnumMember]
		Pending = 0,

		[Description("有效")]
		[EnumMember]
		Active = 1,

		[Description("已失效")]
		[EnumMember]
		InActive = -1,

		[Description("等待提交")]
		[EnumMember]
		Fail = -2
	}
}
