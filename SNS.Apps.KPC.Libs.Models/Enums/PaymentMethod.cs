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
	public enum PaymentMethod
	{
		[Description("线下支付")]
		[EnumMember]
		DirectPay = 0,

		[Description("支付订金")]
		[EnumMember]
		OnlinPay_Deposit = 0x0001,

		[Description("全额支付")]
		[EnumMember]
		OnlinPay_FullCash = 0x0002,

		[Description("支付宝支付")]
		[EnumMember]
		Alipay = 0x0010,

		[Description("微信支付")]
		[EnumMember]
		WXPay = 0x0020,
	}
}
