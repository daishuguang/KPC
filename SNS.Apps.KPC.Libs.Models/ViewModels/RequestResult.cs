using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[Serializable]
	public class RequestResult<T>
	{
		[Newtonsoft.Json.JsonProperty("status")]
		public RequestStatus Status { get; set; }

		[Newtonsoft.Json.JsonProperty("message")]
		public string Message { get; set; }

		[Newtonsoft.Json.JsonProperty("data")]
		public IEnumerable<T> Data { get; set; }
	}

	[Serializable]
	public class RequestResult
	{
		[Newtonsoft.Json.JsonProperty("status")]
		public RequestStatus Status { get; set; }

		[Newtonsoft.Json.JsonProperty("message")]
		public string Message { get; set; }

		[Newtonsoft.Json.JsonProperty("data")]
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