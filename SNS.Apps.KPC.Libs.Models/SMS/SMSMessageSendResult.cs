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
	public class SMSMessageSendResult
	{
		#region "Properties"
		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public string ErrorMsg { get; set; }
		#endregion
	}
}