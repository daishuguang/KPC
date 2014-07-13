using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[Serializable]
	[DataContract]
	public class RequestResult<T>
	{
		[Newtonsoft.Json.JsonProperty("status")]
		[DataMember]
		public RequestStatus Status { get; set; }

		[Newtonsoft.Json.JsonProperty("message")]
		[DataMember]
		public string Message { get; set; }

		[Newtonsoft.Json.JsonProperty("data")]
		[DataMember]
		public IEnumerable<T> Data { get; set; }
	}

	[Serializable]
	[DataContract]
	public class RequestResult
	{
		[Newtonsoft.Json.JsonProperty("status")]
		[DataMember]
		public RequestStatus Status { get; set; }

		[Newtonsoft.Json.JsonProperty("message")]
		[DataMember]
		public string Message { get; set; }

		[Newtonsoft.Json.JsonProperty("data")]
		[DataMember]
		public string Data { get; set; }
	}

	[DataContract]
	public enum RequestStatus
	{
		[EnumMember]
		OK = 0,

		[EnumMember]
		Error = -1
	}
}