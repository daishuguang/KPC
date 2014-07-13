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
	public enum UserPromotionType
	{
		[Description("未设置")]
		[EnumMember]
		NotSet = 0,

		[Description("代理推广")]
		[EnumMember]
		Agent = 1,

		[Description("个人推广")]
		[EnumMember]
		User = 2,

		[Description("平台合作")]
		[EnumMember]
		ExtendChannel = 3
	}
}
