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
	public class CustomerMessage_Voice : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_Voice() { this.MsgType = MessageType.Voice; this.Image = new MessageContent_Image(); }

		public CustomerMessage_Voice(string toUser, string mediaID)
			: this()
		{
			this.ToUser = toUser;
			this.Image.MediaID = mediaID;
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("image")]
		public MessageContent_Image Image { get; set; }
		#endregion
	}
}
