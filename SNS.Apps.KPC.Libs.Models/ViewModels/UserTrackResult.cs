using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserTrackResult
	{
		#region "Properties"
		[DataMember]
		public User User { get; set; }

		[DataMember]
		public Route Route { get; set; }

		[DataMember]
		public Nullable<Point> Position { get; set; }

		[DataMember]
		public Nullable<double> Distance { get; set; }
		#endregion
	}
}
