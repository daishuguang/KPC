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
	public class SMSMessageSendRequest
	{
		#region "Properties"
		[DataMember]
		public IEnumerable<string> Mobiles { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public Nullable<int> Channel { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Mobiles: [{0}], Content: {1}, Channel: {2}", SerializeMobiles(), this.Content, (this.Channel.HasValue ? this.Channel.Value.ToString() : string.Empty));
		}

		public string SerializeMobiles()
		{
			var sbContent = new StringBuilder();

			if (this.Mobiles != null)
			{
				foreach (var m in this.Mobiles)
				{
					sbContent.AppendFormat("{0};", m);
				}
			}

			return sbContent.ToString().TrimEnd(' ', ';');
		}
		#endregion
	}
}
