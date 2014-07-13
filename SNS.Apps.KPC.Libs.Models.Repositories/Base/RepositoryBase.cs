using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Models.Repositories.Base
{
	public abstract class RepositoryBase<T>
		where T : class
	{
		#region "Fields"
		protected const string CNSTR_MEMCACHEKEY_PREFIX = "MemCacheKey";

		// User
		protected const string CNSTR_MEMCACHEKEY_USER_USERID = "ID";
		protected const string CNSTR_MEMCACHEKEY_USER_USERGUID = "UserGUID";
		protected const string CNSTR_MEMCACHEKEY_USER_OPENID = "OpenID";
		protected const string CNSTR_MEMCACHEKEY_USER_MOBILE = "Mobile"; 

		// Route
		protected const string CNSTR_MEMCACHEKEY_ROUTE_ROUTEID = "ID";
		protected const string CNSTR_MEMCACHEKEY_ROUTE_ROUTEGUID = "RouteGUID";

		// RouteMatch
		protected const string CNSTR_MEMCACHEKEY_ROUTEMATCH_ROUTEID = "ID";

		// UserRoute
		//protected const string CNSTR_MEMCACHEKEY_USERROUTE_USERID = "UserID";
		protected const string CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID = "RouteID";

		// UserOrder
		protected const string CNSTR_MEMCACHEKEY_USERORDER_ID = "ID";
		protected const string CNSTR_MEMCACHEKEY_USERORDER_FOLIO = "Folio";

		// UserTrack
		protected const string CNSTR_MEMCACHEKEY_USERTRACK_USERID = "UserID";

		// WXMessage
		protected const string CNSTR_MEMCACHEKEY_WXMESSAGE_USERID = "From_UserID";
		protected const string CNSTR_MEMCACHEKEY_WXMESSAGE_OPENID = "From_OpenID";
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

		#region "Properties"
		public static T Instance { get { return _instance.Value; } }

		protected string[] RepositoryKeys { get; set; }
		#endregion

		#region "Methods"

		#region "Methods: GetMemCacheKey"
		protected virtual string GetMemCacheKey(params object[] suffs)
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
		public virtual TT GetMem<TT>(params object[] suffs)
		{
			return MemCacheWrapper.Get<TT>(GetMemCacheKey(suffs));
		}

		public virtual TT GetMem<TT>(Func<TT> func, params object[] suffs)
		{
			return GetMem<TT>(func, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public virtual TT GetMem<TT>(Func<TT> func, MemCacheExpireMode expireMode, params object[] suffs)
		{
			return GetMem<TT>(func, expireMode, TimeSpan.FromMinutes(Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public virtual TT GetMem<TT>(Func<TT> func, MemCacheExpireMode expireMode, TimeSpan expireIn, params object[] suffs)
		{
			return MemCacheWrapper.Get<TT>(GetMemCacheKey(suffs), func, expireMode, expireIn);
		} 
		#endregion

		#region "Methods: RemoveMem"
		public virtual void RemoveMem(params object[] suffs)
		{
			lock (_lock_Running)
			{
				MemCacheWrapper.Remove(GetMemCacheKey(suffs));
			}
		}

		public virtual void RemoveMemGreedy(params object[] suffs)
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
		public virtual TT SetMem<TT>(TT cacheObj, params object[] suffs)
		{
			return SetMem(cacheObj, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(Libs.Configurations.ConfigStore.CommonSettings.MemCache_ExpiresIn), suffs);
		}

		public virtual TT SetMem<TT>(TT cacheObj, TimeSpan expireIn, params object[] suffs)
		{
			return SetMem(cacheObj, MemCacheExpireMode.Sliding, expireIn, suffs);
		}

		public virtual TT SetMem<TT>(TT cacheObj, MemCacheExpireMode expireMode, TimeSpan expireIn, params object[] suffs)
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

		#region "Methods: RefreshMem"
		public virtual void RefreshMem<TT>(TT instance)
		{
			if (this.RepositoryKeys != null && this.RepositoryKeys.Length > 0)
			{
				var t = instance.GetType();

				foreach (var key in this.RepositoryKeys)
				{
					var p = t.GetProperty(key);
					var v = (p != null) ? (Convert.ToString(p.GetValue(instance, null))) : null;

					if (!string.IsNullOrEmpty(v))
					{
						SetMem(instance, key, v);
					}
				}
			}
		}
		#endregion
		#endregion

        #region "Events"
        void OnMemCachedItemRemoved(CacheEntryRemovedArguments e)
        {
			if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
			{
				DBLogger.Instance.InfoFormat("MemCached Item '{0}' has been removed, with reason '{1}'!", e.CacheItem.Key, e.RemovedReason);
			}
        }
        #endregion
    }
}
