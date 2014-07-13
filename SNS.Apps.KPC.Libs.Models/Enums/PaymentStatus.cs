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
	public enum PaymentStatus
	{
		[Description("等待支付")]
		[EnumMember]
		Pending = 0,

		[Description("支付成功")]
		[EnumMember]
		Success = 1,

		[Description("支付失败")]
		[EnumMember]
		Fail = -1
	}
}
