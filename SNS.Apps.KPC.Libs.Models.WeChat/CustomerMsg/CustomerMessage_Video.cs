using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.WeChat
{
	[DataContract]
	public class CustomerMessage_Video : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_Video() { this.MsgType = MessageType.Video; this.Video = new MessageContent_Video(); }

		public CustomerMessage_Video(string toUser)
			: this()
		{
			this.ToUser = toUser;
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("video")]
		public MessageContent_Video Video { get; set; }
		#endregion
	}
}
