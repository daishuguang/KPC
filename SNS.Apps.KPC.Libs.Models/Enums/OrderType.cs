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
	public enum OrderType
	{
		[Description("我发起的")]
		[EnumMember]
		All = 0,

		[Description("我发起的")]
		[EnumMember]
		RequestByMe = 1,

		[Description("我收到的")]
		[EnumMember]
		AssignToMe = 2
	}
}
