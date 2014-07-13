using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Libs.Models.WeChat
{
	public interface IMessageBase
	{
		#region "Properties"
		string FromUser { get; set; }

		string ToUser { get; set; }

		MessageType MsgType { get; set; }

		Nullable<DateTime> CreateDate { get; set; } 
		#endregion
	}

	[DataContract]
	public abstract class MessageBase : IMessageBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("fromuser", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string FromUser { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("touser", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string ToUser { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("msgtype", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public virtual MessageType MsgType { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("createtime", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.JavaScriptDateTimeConverter))]
		public virtual Nullable<DateTime> CreateDate { get; set; }
		#endregion
	}

	[DataContract]
	public abstract class MessageContentBase { }

	[DataContract]
	public class MessageContent_Image : MessageContentBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("media_id", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MediaID { get; set; }
		#endregion
	}

	[DataContract]
	public class MessageContent_Music : MessageContentBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("title", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Title { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Description { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("musicurl", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MediaUrl { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("hqmusicurl", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MusicUrl_HQ { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("thumb_media_id", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MediaID_Thumb { get; set; }
		#endregion
	}

	[DataContract]
	public class MessageContent_News : MessageContentBase
	{
		#region "Constructs"
		public MessageContent_News() { this.Articles = new List<MessageContent_News_Article>(); } 
		#endregion

		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("articles", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual List<MessageContent_News_Article> Articles { get; set; }
		#endregion
	}

	public class MessageContent_News_Article
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("title", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Title { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Description { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("url", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Url { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("picurl", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string PicUrl { get; set; }
		#endregion
	}

	[DataContract]
	public class MessageContent_Text : MessageContentBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("content", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Content { get; set; } 
		#endregion
	}

	[DataContract]
	public class MessageContent_Video : MessageContentBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("title", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Title { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string Description { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("media_id", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MediaID { get; set; }
		#endregion
	}

	[DataContract]
	public class MessageContent_Voice : MessageContentBase
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("media_id", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public virtual string MediaID { get; set; }
		#endregion
	}
}
