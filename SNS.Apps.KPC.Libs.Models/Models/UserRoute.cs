using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserRoute
	{
		#region "Constructs"
		public UserRoute() { }

		public UserRoute(DataStores.sp_Load_UserRoute_By_RouteID_Result entity)
		{
			this.User = new User(entity);
			this.Route = new Route(entity);
			this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
		}

		public UserRoute(DataStores.sp_Load_UserRoute_By_RouteIDs_Result entity)
		{
			this.User = new User(entity);
			this.Route = new Route(entity);
			this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
		}

		public UserRoute(DataStores.sp_Load_UserRoute_By_UserID_Result entity)
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
		public Nullable<UserRole> UserRole { get; set; }
		#endregion
	}
}
