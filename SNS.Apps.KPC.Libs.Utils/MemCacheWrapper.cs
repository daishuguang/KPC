using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Libs.Utils
{
	public static class MemCacheWrapper
	{
		#region "Fields"
		static readonly Lazy<MemoryCache> _cache = new Lazy<MemoryCache>(() => MemoryCache.Default);
		#endregion

		#region "Properties"
		public static MemoryCache Cache { get { return _cache.Value; } }
		#endregion

		#region "Methods"
		public static T Get<T>(string cacheKey)
		{
			return Get<T>(cacheKey, null, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(ConfigStore.CommonSettings.MemCache_ExpiresIn));
		}

		public static T Get<T>(string cacheKey, Func<T> func)
		{
			return Get<T>(cacheKey, func, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(ConfigStore.CommonSettings.MemCache_ExpiresIn));
		}

		public static T Get<T>(string cacheKey, Func<T> func, TimeSpan expireIn)
		{
			return Get<T>(cacheKey, func, MemCacheExpireMode.Sliding, expireIn);
		}

		public static T Get<T>(string cacheKey, Func<T> func, MemCacheExpireMode expireMode, TimeSpan expireIn)
		{
			var obj = Cache.Get(cacheKey);

			if (obj == null && func != null)
			{
				var policy = new CacheItemPolicy();

				policy.Priority = CacheItemPriority.Default;
				//policy.RemovedCallback = new CacheEntryRemovedCallback(OnCacheItem_Removed);

				switch (expireMode)
				{
 					case MemCacheExpireMode.Absolute:
						policy.AbsoluteExpiration = DateTime.Now.AddMinutes(expireIn.TotalMinutes);
						break;
					case MemCacheExpireMode.Sliding:
						policy.SlidingExpiration = expireIn;
						break;
				}

				obj = Set<T>(cacheKey, func(), policy);
			}

			return (obj != null) ? ((T)obj) : (default(T));
		}

		public static T Set<T>(string cacheKey, T cacheObj)
		{
			if (ConfigStore.CommonSettings.MemCache_Enabled)
			{
				return Set(cacheKey, cacheObj, null);
			}
			else
			{
				return cacheObj;
			}
		} 

		public static T Set<T>(string cacheKey, T cacheObj, CacheItemPolicy policy)
		{
			if (ConfigStore.CommonSettings.MemCache_Enabled)
			{
				if (cacheObj != null)
				{
					var p = default(CacheItemPolicy);

					if (policy == null)
					{
						p = new CacheItemPolicy();

						p.Priority = CacheItemPriority.Default;
						p.SlidingExpiration = TimeSpan.FromMinutes(ConfigStore.CommonSettings.MemCache_ExpiresIn);
					}
					else
					{
						p = policy;
					}

					Cache.Set(new CacheItem(cacheKey, cacheObj), p);

					return Get<T>(cacheKey);
				}
				else
				{
					Remove(cacheKey);
				}

				return default(T);
			}
			else
			{
				return cacheObj;
			}
		}

		public static void Remove(string cacheKey, bool isGreedy = false)
		{
			if (isGreedy)
			{
				var keys = Cache.Where(p => p.Key.StartsWith(cacheKey)).Select(p => p.Key);

				foreach (var key in keys)
				{
					Cache.Remove(key);
				}
			}
			else
			{
				Cache.Remove(cacheKey);
			}
		}
		#endregion
	}

	public enum MemCacheExpireMode
	{
		Absolute,
		Sliding
	}
}
