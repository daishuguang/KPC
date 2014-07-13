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
	public class CustomerMessage_News : CustomerMessageBase
	{
		#region "Constructs"
		public CustomerMessage_News() { this.MsgType = MessageType.News; this.News = new MessageContent_News(); }

		public CustomerMessage_News(string toUser) : this() 
		{
			this.ToUser = toUser;
		}
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("news")]
		public MessageContent_News News { get; set; }
		#endregion
	}
}
