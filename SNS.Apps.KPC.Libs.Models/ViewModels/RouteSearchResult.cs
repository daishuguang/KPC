using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
    public class RouteSearchResult
    {
		#region "Constructs"
		public RouteSearchResult() { }

		public RouteSearchResult(UserRoute entity)
		{
			this.User = entity.User;
			this.Route = entity.Route;
			this.UserRole = entity.UserRole.HasValue ? entity.UserRole.Value : Models.UserRole.Passenger;
		}

		public RouteSearchResult(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			this.User = new User(entity);
			this.Route = new Route(entity);
			this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
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
		public double Rate_Distance { get; set; }

		[DataMember]
		public double Rate_DateTime { get; set; }

		[DataMember]
		public DateTime Sort_DateTime { get; set; }
		#endregion
    }
}
