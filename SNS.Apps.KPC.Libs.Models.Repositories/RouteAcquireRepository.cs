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
	public sealed class RouteAcquireRepository : Common.RepositoryBase<RouteAcquireRepository>
	{
		#region "Constructs"
		private RouteAcquireRepository() { }
		#endregion

		#region "Methods: Get"
		public RouteAcquire Get(long routeID, bool inForce = false)
		{
			var ra = default(RouteAcquire);

			if (inForce)
			{
				ra = Get_FromDB(routeID);

				MemCacheWrapper.Set(GetMemCacheKey("RouteID", routeID), ra);
			}
			else
			{
				ra = MemCacheWrapper.Get<RouteAcquire>(GetMemCacheKey("RouteID", routeID), () =>
				{
					return Get_FromDB(routeID);
				});
			}

			return ra;
		}

		public RouteAcquire Get(Guid routeID, bool inForce = false)
		{
			var ra = default(RouteAcquire);

			if (inForce)
			{
				var route = RouteRepository.Instance.Get(routeID);

				if (route != null)
				{
					ra = Get(route.ID, inForce);

					MemCacheWrapper.Set(GetMemCacheKey("RouteGUID", routeID), ra);
				}
			}
			else
			{
				ra = MemCacheWrapper.Get<RouteAcquire>(GetMemCacheKey("RouteGUID", routeID), () =>
				{
					var route = RouteRepository.Instance.Get(routeID);

					if (route != null)
					{
						return Get(route.ID, inForce);
					}

					return null;
				});
			}

			return ra;
		} 

		RouteAcquire Get_FromDB(long routeID)
		{
			using (var ctx = new DataStores.DataModels())
			{
				var lst = new List<RouteAcquireItem>();
				var results = ctx.tbl_Route_Acquire.Where(p => p.RouteID == routeID);

				if (results != null && results.Count() > 0)
				{
					foreach (var result in results)
					{
						lst.Add(new RouteAcquireItem(result));
					}
				}

				if (lst.Count > 0)
				{
					var instance = new RouteAcquire { RouteID = routeID };

					lst.ForEach(p =>
					{
						instance.Acquires.Add(new RouteAcquireItem { RouteID = p.RouteID, AcquireType = p.AcquireType, Status = p.Status });
					});

					return instance;
				}

				return null;
			}
		}
		#endregion
	}
}
