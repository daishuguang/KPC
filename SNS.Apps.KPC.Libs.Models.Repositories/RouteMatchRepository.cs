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
	public sealed class RouteMatchRepository : Base.RepositoryBase<RouteMatchRepository>
	{
		#region "Constructx"
		private RouteMatchRepository() { this.RepositoryKeys = new string[] { CNSTR_MEMCACHEKEY_ROUTEMATCH_ROUTEID }; }
		#endregion

		#region "Methods: Load"
		public RouteMatch Load(long routeID, bool inForce = false)
		{
			var instance = default(RouteMatch);

			if (inForce)
			{
				instance = Load_FromDB(routeID);

				SetMem(instance, CNSTR_MEMCACHEKEY_ROUTEMATCH_ROUTEID, routeID);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Load_FromDB(routeID);
				},
				CNSTR_MEMCACHEKEY_ROUTEMATCH_ROUTEID, routeID);
			}

			return instance;
		}

		RouteMatch Load_FromDB(long routeID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var results = ctx.tbl_Route_Match.Where(p => p.RouteID == routeID);

				if (results != null)
				{
					return new RouteMatch(results);
				}
			}

			return new RouteMatch();
		}
		#endregion
	}
}
