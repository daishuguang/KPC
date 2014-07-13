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
	public enum OrderStatus
	{
		[EnumMember]
		All = 200,

		[Description("等待支付")]
		[EnumMember]
		PendingForPayment = 0,

		[Description("已付订金")] 
		[EnumMember]
		FinishToPayment_Deposit = 1,

		[Description("已付余款")]
		[EnumMember]
		FinishToPayment_Residue = 2,

		[Description("已支付")]
		[EnumMember]
		FinishToPayment_FullCash = 3,

		[Description("已完成")]
		[EnumMember]
		Completed = 100,

		[Description("已过期")]
		[EnumMember]
		Expired = -1,

		[Description("已取消")]
		[EnumMember]
		Cancelled = -2,

		[Description("已申请退款")]
		[EnumMember]
		PendingForRefund = -3,

		[Description("已退款")]
		[EnumMember]
		FinishToRefund = -4,
	}
}