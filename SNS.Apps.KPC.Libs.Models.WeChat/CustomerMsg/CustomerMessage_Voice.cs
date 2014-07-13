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
	public class CustomerMessage_Image : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_Image() { this.MsgType = MessageType.Voice; this.Voice = new MessageContent_Voice(); }

		public CustomerMessage_Image(string toUser, string mediaID)
			: this()
		{
			this.ToUser = toUser;
			this.Voice.MediaID = mediaID;
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("voice")]
		public MessageContent_Voice Voice { get; set; }
		#endregion
	}
}
