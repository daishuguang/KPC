using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace SNS.Apps.KPC.Libs.Models.WeChat
{
	[DataContract]
	public enum MessageType
	{
		[EnumMember(Value = "image")]
		Image,

		[EnumMember(Value = "music")]
		Music,

		[EnumMember(Value = "news")]
		News,

		[EnumMember(Value = "text")]
		Text,

		[EnumMember(Value = "video")]
		Video,

		[EnumMember(Value = "voice")]
		Voice
	}
}
