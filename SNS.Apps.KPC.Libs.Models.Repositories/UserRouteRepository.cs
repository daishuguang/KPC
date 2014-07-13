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
	public sealed class UserRouteRepository : Base.RepositoryBase<UserRouteRepository>
	{
		#region "Constructs"
		private UserRouteRepository() { }
		#endregion

		#region "Methods: GetByRouteID"
		public UserRoute GetByRouteID(long routeID, bool inForce = false)
		{
			var instance = default(UserRoute);

			if (inForce)
			{
				instance = GetByRouteID_FromDB(routeID);

				SetMem(instance, CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, instance.Route.ID);
			}
			else
			{
				instance = GetMem(() =>
				{
					return GetByRouteID_FromDB(routeID);
				}, CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, routeID);
			}

			return instance;
		}

		public UserRoute GetByRouteID(Guid routeID, bool inForce = false)
		{
			var routeInstance = RouteRepository.Instance.Get(routeID, inForce);
			
			return (routeInstance != null) ? (GetByRouteID(routeInstance.ID, inForce)) : (null);
		}

		UserRoute GetByRouteID_FromDB(long routeID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var results = ctx.sp_Load_UserRoute_By_RouteID(routeID);
				var result = (results != null) ? (results.FirstOrDefault()) : (null);

				if (result != null)
				{
					return new UserRoute(result);
				}

				return null;
			}
		}
		#endregion

		#region "Methods: GetByUserID"
		public UserRoute GetByUserID(long userID, bool inForce = false)
		{
			var urs = LoadByUserID(userID, 0, 1, inForce);

			if (urs != null)
			{
				return urs.FirstOrDefault();
			}

			return null;
		}

		public UserRoute GetByUserID(Guid userID, bool inForce = false)
		{
			var urs = LoadByUserID(userID, 0, 1, inForce);

			if (urs != null)
			{
				return urs.FirstOrDefault();
			}

			return null;
		}

		public UserRoute GetByUserID(string openID, bool inForce = false)
		{
			var urs = LoadByUserID(openID, 0, 1, inForce);

			if (urs != null)
			{
				return urs.FirstOrDefault();
			}

			return null;
		} 
		#endregion

		#region "Methods: LoadByUserID"
		public IEnumerable<UserRoute> LoadByUserID(long userID, int page = 0, int count = 10, bool inForce = false)
		{
			return LoadByUserID_FromDB(userID, page, count);
		}

		public IEnumerable<UserRoute> LoadByUserID(Guid userID, int page = 0, int count = 10, bool inForce = false)
		{
			var user = UserRepository.Instance.Get(userID, inForce);

			return (user != null) ? (LoadByUserID(user.ID, page, count, inForce)) : (null);
		}

		public IEnumerable<UserRoute> LoadByUserID(string openID, int page = 0, int count = 10, bool inForce = false)
		{
			var user = UserRepository.Instance.Get(openID, inForce);

			return (user != null) ? (LoadByUserID(user.ID, page, count, inForce)) : (null);
		}

		IList<UserRoute> LoadByUserID_FromDB(long userID, int page = 0, int count = 10)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var lst = new List<UserRoute>();
				var results = ctx.sp_Load_UserRoute_By_UserID(userID, page, count);

				if (results != null)
				{
					foreach (var result in results)
					{
						lst.Add(new UserRoute (result));
					}
				}

				return lst;
			}
		}
		#endregion

		#region "Methods: LoadByRouteIDs"
		public IEnumerable<UserRoute> LoadByRouteIDs(IEnumerable<long> routeIDs, bool inForce = false)
		{
			var lst = new List<UserRoute>();

			foreach (var routeID in routeIDs)
			{
				var instance = GetByRouteID(routeID, inForce);

				if (instance != null)
				{
					lst.Add(instance);
				}
			}

			return lst.ToArray();
		}

		public IEnumerable<UserRoute> LoadByRouteIDs(IEnumerable<Guid> routeIDs, bool inForce = false)
		{
			var lst = new List<UserRoute>();

			foreach (var routeID in routeIDs)
			{
				var instance = GetByRouteID(routeID, inForce);

				if (instance != null)
				{
					lst.Add(instance);
				}
			}

			return lst.ToArray();
		}

		public IEnumerable<UserRoute> LoadByRouteIDs(string routeIDs)
		{
			var lst = new List<UserRoute>();

			using (var ctx = new DataStores.KPCDataModels())
			{
				var results = ctx.sp_Load_UserRoute_By_RouteIDs(routeIDs);

				if (results != null)
				{
					foreach(var result in results)
					{
						var ur = new UserRoute(result);

						if (ur != null)
						{
							// Set to MemeCache
							SetMem(ur, CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, ur.Route.ID);

							lst.Add(ur);
						}
						//else if (ConfigStore.CommonSettings.Trace_Mode)
						//{
						//	DBLogger.Instance.WarnFormat("Invalid UR: UserID: {0}, RouteID: {1}!", result.UserID, result.RouteID);
						//}
					}
				}
			}

			return lst;
		} 
		#endregion

		#region "Methods: LoadNewest"
		public IEnumerable<UserRouteResult> LoadNewest(string city, int page = 0, int count = 10, bool inForce = false)
		{
			return GetMem(() => { return LoadNewest_FromDB(city, page, count); }, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), ((!string.IsNullOrEmpty(city)) ? (city) : ("All")), page, count);
		}

		public IEnumerable<UserRouteResult> LoadNewest4Disp(string city, bool inForce = false)
		{
			return GetMem(() => { return LoadNewest4Disp_FromDB(city); }, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), ((!string.IsNullOrEmpty(city)) ? (city) : ("All")));
		}

		IEnumerable<UserRouteResult> LoadNewest_FromDB(string city, int page = 0, int count = 10)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var lst = new List<UserRouteResult>();
				var results = ctx.sp_Load_UserRoute_Newest(city, page, count);

				if (results != null)
				{
					foreach (var result in results)
					{
						lst.Add(new UserRouteResult(result));
					}
				}

				return lst.ToArray();
			}
		}

		IEnumerable<UserRouteResult> LoadNewest4Disp_FromDB(string city)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var lst = new List<UserRouteResult>();
				var results = ctx.sp_Load_UserRoute_Newest_Disp(city);

				if (results != null)
				{
					foreach (var result in results)
					{
						lst.Add(new UserRouteResult(result));
					}
				}

				return lst.ToArray();
			}
		}
		#endregion
	}
}
