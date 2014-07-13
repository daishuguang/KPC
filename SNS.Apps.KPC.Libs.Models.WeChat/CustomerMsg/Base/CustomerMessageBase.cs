using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.WeChat
{
	public interface ICustomerMessageBase
	{
		#region "Properties"
		string ToUser { get; set; }

		MessageType MsgType { get; set; }
		#endregion
	}

	[DataContract]
	[KnownType(typeof(CustomerMessage_Image))]
	[KnownType(typeof(CustomerMessage_Music))]
	[KnownType(typeof(CustomerMessage_News))]
	[KnownType(typeof(CustomerMessage_Text))]
	[KnownType(typeof(CustomerMessage_Video))]
	[KnownType(typeof(CustomerMessage_Voice))]
	public abstract class CustomerMessageBase : MessageBase, ICustomerMessageBase
	{
		#region "Properties"
		[IgnoreDataMember]
		private new string FromUser { get; set; }

		[IgnoreDataMember]
		private new DateTime? CreateDate { get; set; }
		#endregion

		#region "Methods"
		public static T CreateCustomerMessageInstance<T>()
			where T : CustomerMessageBase
		{
			return Activator.CreateInstance<T>();
		} 
		#endregion
	}

	[DataContract]
	public class CustomerMessage_RetMsg
	{
		#region "Properties"
		[DataMember]
		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[DataMember]
		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; }
		#endregion
	}
}
