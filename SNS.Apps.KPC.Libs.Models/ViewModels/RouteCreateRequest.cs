using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class RouteCreateRequest
	{
		#region "Constructs"
		public RouteCreateRequest() { }

		public RouteCreateRequest(User user, Route route, UserRole userRole)
		{
			this.User = user;
			this.Route = route;
			this.UserRole = userRole;
		}
		#endregion

		#region "Properties"
		[DataMember]
		public User User { get; set; }

		[DataMember]
		public Route Route { get; set; }

		[DataMember]
		public UserRole UserRole { get; set; } 
		#endregion
	}
}
