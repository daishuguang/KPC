using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models.ViewModels
{
	[DataContract]
	public abstract class PagedRequest
	{
		#region "Properties"
		[DataMember]
		public Nullable<int> Page { get; set; }

		[DataMember]
		public Nullable<int> Count { get; set; } 
		#endregion
	}
}
