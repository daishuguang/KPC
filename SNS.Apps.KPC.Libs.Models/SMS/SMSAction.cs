using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.SMS
{
	[DataContract]
	public enum SMSAction
	{
		[EnumMember]
		GetBalance = 0,

		[EnumMember]
		SendSMS = 1,

		[EnumMember]
		RecvSmsEx = 2
	}
}
