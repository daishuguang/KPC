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
	public class RouteMatrix
	{
		#region "Fields"
		IList<RouteMatrixItem> _lst = new List<RouteMatrixItem>(); 
		#endregion

		#region "Constructs"
		public RouteMatrix(IEnumerable<DataStores.sp_Load_Available_Routes_Result> results)
		{
			foreach(var entity in results)
			{
				this._lst.Add(new RouteMatrixItem(entity));
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public IEnumerable<RouteMatrixItem> Items { get { return _lst.ToArray(); } }
		#endregion

		#region "Methods"
		public RouteMatrix Add(UserRoute ur)
		{
			var item = Get(ur.Route.ID);

			if (item == null)
			{
				this._lst.Add(new RouteMatrixItem(ur));
			}
			else
			{
				this._lst.Remove(item);
				this._lst.Add(new RouteMatrixItem(ur));
			}

			return this;
		}

		public RouteMatrix Remove(long routeID)
		{
			var item = Get(routeID);

			if (item != null)
			{
				this._lst.Remove(item);
			}

			return this;
		}

		public RouteMatrixItem Get(long routeID)
		{
			return this._lst.SingleOrDefault(p => p.Route.ID == routeID);
		} 
		#endregion
	}

	[DataContract]
	public class RouteMatrixItem
	{
		#region "Constructs"
		public RouteMatrixItem(DataStores.sp_Load_Available_Routes_Result entity)
		{
			this.UserID = entity.UserID;
			this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
			this.Route = new Route(entity);
		}

		public RouteMatrixItem(UserRoute entity)
		{
			this.UserID = entity.User.ID;
			this.UserRole = entity.UserRole.HasValue ? entity.UserRole.Value : Models.UserRole.Passenger;
			this.Route = new Models.Route();

			Libs.Utils.CommonUtility.CopyTo(entity.Route, this.Route);
		}

		public RouteMatrixItem(RouteSearchFilter filter)
		{
			this.Route = new Route(filter);
			this.UserID = (filter.UserID != null && filter.UserID.HasValue ? filter.UserID.Value : 0);
			this.UserRole = (filter.UserRole != null && filter.UserRole.HasValue ? filter.UserRole.Value : Models.UserRole.Passenger);

			if (filter.LocationFilter.To_Point == null || !filter.LocationFilter.To_Point.HasValue || !filter.LocationFilter.To_Point.Value.IsAvailable)
			{
				this.Route.RouteType = RouteType.All;
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long UserID { get; set; }

		[DataMember]
		public UserRole UserRole { get; set; }

		[DataMember]
		public Route Route { get; set; }
		#endregion
	}
}
