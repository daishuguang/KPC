using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class RouteMatchResult
	{
		#region "Constructs"
		public RouteMatchResult() { }

		public RouteMatchResult(UserRoute ur, double? rate_time, double? rate_dist)
		{
			this.User = ur.User;
			this.Route = ur.Route;
			this.UserRole = ur.UserRole.HasValue ? ur.UserRole.Value : Models.UserRole.Passenger;
			this.Rate = new RouteMatchRate { Rate_DateTime = rate_time, Rate_Distance = rate_dist };
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public User User { get; set; }

		[DataMember]
		public Route Route { get; set; }

		[DataMember]
		public UserRole UserRole { get; set; }

		[DataMember]
		public RouteMatchRate Rate { get; set; }
		#endregion
	}

	[DataContract]
	public class RouteMatchRate
	{
		#region "Properties"
		[DataMember]
		public Nullable<DateTime> Sort_DateTime { get; set; }

		[DataMember]
		public Nullable<double> Rate_DateTime { get; set; }

		[DataMember]
		public Nullable<double> Rate_Distance { get; set; } 
		#endregion
	}
}
