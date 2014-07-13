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
	public class CustomerMessage_Text : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_Text() { this.MsgType = MessageType.Text; this.Text = new MessageContent_Text(); }

		public CustomerMessage_Text(string toUser, string content)
			: this()
		{
			this.ToUser = toUser;
			this.Text.Content = content.Trim();
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("text")]
		public MessageContent_Text Text { get; set; }
		#endregion
	}
}
