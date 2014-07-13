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
	public class SMSMessage
	{
		#region "Properties"
		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public string Content { get; set; }
		#endregion
	}
}
