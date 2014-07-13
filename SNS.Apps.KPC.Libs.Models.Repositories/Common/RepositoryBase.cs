using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories.Common
{
	public abstract class RepositoryBase<T>
		where T : class
	{
		#region "Fields"
		protected const string CNSTR_MEMCACHEKEY_PREFIX = "MemCacheKey";

		// User
		protected const string CNSTR_MEMCACHEKEY_USER_USERID = "UserID";
		protected const string CNSTR_MEMCACHEKEY_USER_USERGUID = "UserGUID";
		protected const string CNSTR_MEMCACHEKEY_USER_USEROPENID = "OpenID";
		protected const string CNSTR_MEMCACHEKEY_USER_USERMOBILE = "Mobile"; 

		// Route
		protected const string CNSTR_MEMCACHEKEY_ROUTE_ROUTEID = "RouteID";
		protected const string CNSTR_MEMCACHEKEY_ROUTE_ROUTEGUID = "RouteGUID";
		protected const string CNSTR_MEMCACHEKEY_ROUTE_LATEST_USERGUID = "Latest_UserGUID";

		// UserRoute
		protected const string CNSTR_MEMCACHEKEY_USERROUTE_USERID = "UserID";
		protected const string CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID = "RouteID";

		// UserTrack
		protected const string CNSTR_MEMCACHEKEY_USERTRACK_USERID = "UserID";
		#endregion

		#region "Fields"
		static Lazy<T> _instance = new Lazy<T>(() =>
		{
			var ctor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, new ParameterModifier[] { });

			return (T)(ctor.Invoke(null));
		});

		static object _lock_Running = new object();
		//static volatile bool _isRunning = false;
		#endregion

		#region "Methods"
		public static T Instance { get { return _instance.Value; } }
		#endregion

		#region "Static Methods"

		#region "Methods: GetMemCacheKey"
		protected static string GetMemCacheKey(params object[] suffs)
		{
			var prefix = string.Empty;
			var typefix = typeof(T).Name;

			typefix = typefix.Substring(typefix.LastIndexOf(".") + 1);

			if (suffs != null && suffs.Length > 0)
			{
				foreach (var suff in suffs)
				{
					if (suff != null)
					{
						prefix += suff.ToString() + "_";
					}
				}

				prefix = prefix.TrimEnd('_');
			}

			return (string.Format("{0}_{1}_{2}", CNSTR_MEMCACHEKEY_PREFIX, typefix, prefix)).TrimEnd('_');
		} 
		#endregion

		#region "Methods: GetMem"
		public TT GetMem<TT>(params object[] suffs)
		{
			return MemCacheWrapper.Get<TT>(GetMemCacheKey(suffs));
		}

		public TT GetMem<TT>(Func<TT> func, params object[] suffs)
		{
			return GetMem<TT>(func, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public TT GetMem<TT>(Func<TT> func, MemCacheExpireMode expireMode, params object[] suffs)
		{
			return GetMem<TT>(func, expireMode, TimeSpan.FromMinutes(Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public TT GetMem<TT>(Func<TT> func, MemCacheExpireMode expireMode, TimeSpan expireIn, params object[] suffs)
		{
			return MemCacheWrapper.Get<TT>(GetMemCacheKey(suffs), func, expireMode, expireIn);
		} 
		#endregion

		#region "Methods: RemoveMem"
		public void RemoveMem(params object[] suffs)
		{
			lock (_lock_Running)
			{
				MemCacheWrapper.Remove(GetMemCacheKey(suffs));
			}
		}

		public void RemoveMemGreedy(params object[] suffs)
		{
			var cacheKey = GetMemCacheKey();

			if (suffs != null && suffs.Length > 0)
			{
				lock (_lock_Running)
				{
					foreach (var suff in suffs)
					{
						MemCacheWrapper.Remove(string.Format("{0}_{1}", cacheKey, suff), true);
					}
				}
			}
		} 
		#endregion

		#region "Methods: SetMem"
		public TT SetMem<TT>(TT cacheObj, params object[] suffs)
		{
			return SetMem(cacheObj, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(Libs.Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public TT SetMem<TT>(TT cacheObj, TimeSpan expireIn, params object[] suffs)
		{
			return SetMem(cacheObj, MemCacheExpireMode.Sliding, expireIn, suffs);
		}

		public TT SetMem<TT>(TT cacheObj, MemCacheExpireMode expireMode, TimeSpan expireIn, params object[] suffs)
		{
			lock (_lock_Running)
			{
				var policy = new CacheItemPolicy();

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	policy.RemovedCallback = new CacheEntryRemovedCallback(OnMemCachedItemRemoved);
				//}

				switch (expireMode)
				{
					case MemCacheExpireMode.Absolute:
						policy.AbsoluteExpiration = DateTime.Now.AddMinutes(expireIn.TotalMinutes);
						break;
					case MemCacheExpireMode.Sliding:
						policy.SlidingExpiration = expireIn;
						break;
				}

				return MemCacheWrapper.Set<TT>(GetMemCacheKey(suffs), cacheObj, policy);
			}
		} 
		#endregion

		#endregion

        #region "Events"
        void OnMemCachedItemRemoved(CacheEntryRemovedArguments e)
        {
            Utils.FileLogger.CurrentLogger.InfoFormat("MemCached Item '{0}' has been removed, with reason '{1}'!", e.CacheItem.Key, e.RemovedReason);
        }
        #endregion
    }
}
