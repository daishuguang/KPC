using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class RouteRepository : Base.RepositoryBase<RouteRepository>
	{
		#region "Constructs"
		private RouteRepository() { this.RepositoryKeys = new string[] { CNSTR_MEMCACHEKEY_ROUTE_ROUTEID, CNSTR_MEMCACHEKEY_ROUTE_ROUTEGUID }; }
		#endregion

		#region "Public Methods"
		#region "Methods: Get"
		public Route Get(long routeID, bool inForce = false)
		{
			var instance = default(Route);

			if (inForce)
			{
				instance = Get_FromDB(routeID);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() => 
				{
					return Get_FromDB(routeID);
				}, 
				CNSTR_MEMCACHEKEY_ROUTE_ROUTEID, routeID);
			}

			return instance;
		}

		public Route Get(Guid routeID, bool inForce = false)
		{
			var instance = default(Route);

			if (inForce)
			{
				instance = Get_FromDB(routeID);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(routeID);
				},
				CNSTR_MEMCACHEKEY_ROUTE_ROUTEGUID, routeID);
			}

			return instance;
		}

		Route Get_FromDB(long routeID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				return (new Route(ctx.tbl_Route.FirstOrDefault(p => p.ID == routeID)));
			}
		}

		Route Get_FromDB(Guid routeID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var uid = routeID.ToString();

				return (new Route(ctx.tbl_Route.FirstOrDefault(p => string.Compare(p.RouteGUID, uid, true) == 0)));
			}
		} 
		#endregion

		#region "Methods: Load"
		public IEnumerable<Route> Load(IEnumerable<long> ids, bool inForce = false)
		{
			var lst = new List<Route>();

			foreach (var id in ids)
			{
				var instance = Get(id, inForce);

				if (instance != null)
				{
					lst.Add(instance);
				}
			}

			return lst.ToArray();
		}

		public IEnumerable<Route> Load(IEnumerable<Guid> ids, bool inForce = false)
		{
			var lst = new List<Route>();

			foreach (var id in ids)
			{
				var instance = Get(id, inForce);

				if (instance != null)
				{
					lst.Add(instance);
				}
			}

			return lst.ToArray();
		} 
		#endregion

		#region "Methods: GetLatest"
		//public Route GetLatest(Guid userID)
		//{
		//	var instance = GetLatest_FromDB(userID);

		//	RefreshMem(instance);

		//	return instance;
		//}

		//public Route GetLatest_FromDB(Guid userID)
		//{
		//	using (var ctx = new DataStores.KPCDataModels())
		//	{
		//		var uID = userID.ToString();
		//		var results = ctx.sp_Get_Latest_RouteID_By_UserGUID(uID);

		//		if (results != null)
		//		{
		//			var rID = results.FirstOrDefault();

		//			return (rID.HasValue) ? (Get(rID.Value, true)) : (null);
		//		}

		//		return null;
		//	}
		//}
		#endregion

		#region "Methods: SetStatus"
		public void SetStatus(long routeID, RouteStatus status)
		{
			var instance = Get(routeID);

			if (instance != null)
			{
				// Set from DB
				SetStatus_ToDB(instance, status);

				// Remove from MemCache
				SetStatus_ToMem(instance, status);
			}
		}

		public void SetStatus(Guid routeID, RouteStatus status)
		{
			var instance = Get(routeID);

			if (instance != null)
			{
				// Set from DB
				SetStatus_ToDB(instance, status);

				// Remove from MemCache
				SetStatus_ToMem(instance, status);
			}
		}

		void SetStatus_ToDB(Route route, RouteStatus status)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				ctx.sp_Set_Route_Status(route.ID, (int)status);
			}
		}

		void SetStatus_ToMem(Route route, RouteStatus status)
		{
			switch (status)
			{
				case RouteStatus.Available:
					break;
				case RouteStatus.Inavailable:
					{
						// Mem: Route
						RemoveMem(CNSTR_MEMCACHEKEY_ROUTE_ROUTEID, route.ID);
						RemoveMem(CNSTR_MEMCACHEKEY_ROUTE_ROUTEGUID, route.RouteGUID);

						// Mem: UserRoute
						var urInstance = UserRouteRepository.Instance.GetByRouteID(route.ID);

						if (urInstance != null)
						{
							UserRouteRepository.Instance.RemoveMem(CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, urInstance.Route.ID);
						}
					}
					break;
			}
		}
		#endregion
		#endregion
	}
}
