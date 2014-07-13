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
	public enum UserStatus
	{
		[Description("加关注")]
		[EnumMember]
		Subscribed = 0,

		[Description("已注册")]
		[EnumMember]
		Registered = 1,

		[Description("已取消关注")]
		[EnumMember]
		UnSubscribed = -1
	}
}
