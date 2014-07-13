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
	public class CustomerMessage_Music : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_Music() { this.MsgType = MessageType.Music; this.Music = new MessageContent_Music(); }

		public CustomerMessage_Music(string toUser)
			: this()
		{
			this.ToUser = toUser;
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("music")]
		public MessageContent_Music Music { get; set; }
		#endregion
	}
}
