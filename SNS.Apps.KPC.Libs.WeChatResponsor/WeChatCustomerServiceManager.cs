using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.WeChatResponsor
{
	public class WeChatCustomerServiceManager
	{
		#region "Fields"
		static Lazy<WeChatCustomerServiceManager> _instance = new Lazy<WeChatCustomerServiceManager>(() =>
		{
			if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
			{
				DBLogger.Instance.Info("WeChatCustomerServiceManager is initialing...");
			}

			var t = typeof(WeChatCustomerServiceManager);
			var ctor = t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, new ParameterModifier[] { });

			return (WeChatCustomerServiceManager)ctor.Invoke(new object[] { });
		});

		//static object _lock_Running = new object();
		#endregion

		#region "Fields"
		ConcurrentDictionary<string, WeChatCustomerServiceModel> _dicModels = new ConcurrentDictionary<string, WeChatCustomerServiceModel>();
		Timer _timer = null;
		#endregion

		#region "Constructs"
		private WeChatCustomerServiceManager()
		{
			if (_timer == null)
			{
				_timer = new Timer(new TimerCallback(Timer_OnTick), null, 1000, 1000 * 60 * 10);
			}
		}
		#endregion

		#region "Properties"
		public static WeChatCustomerServiceManager Instance { get { return _instance.Value; } }
		#endregion

		#region "Events"
		void Timer_OnTick(object state)
		{
			if (_dicModels.Count > 0)
			{
				var lstExpired = new List<string>();

				foreach(var item in _dicModels.Where(p => p.Value.ExpireDate.Value.CompareTo(DateTime.Now) <= 0))
				{
					lstExpired.Add(item.Key);
				}

				if (lstExpired.Count > 0)
				{
					var sbContent = new StringBuilder();

					lstExpired.ForEach(p => 
					{
						var flag = false;
						var item = default(WeChatCustomerServiceModel);

						while(!_dicModels.TryRemove(p, out item))
						{
							System.Threading.Thread.Sleep(100);
						}

						sbContent.AppendFormat("{0}, ", p); 
					});

					if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
					{
						DBLogger.Instance.InfoFormat("User(s) {0} left from conversation list, {1} user(s) remain(s).", sbContent.ToString().TrimEnd(' ', ','), _dicModels.Count);
					}
				}
			}
		}
		#endregion

		#region "Methods"
		public void Add(string openID)
		{
			if (IsExist(openID))
			{
				(_dicModels.FirstOrDefault(p => string.Compare(p.Key, openID, StringComparison.InvariantCultureIgnoreCase) == 0)).Value.Refresh();

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	Utils.DBLogger.Instance.InfoFormat("Use {0} extend in conversation list, {1} user(s) remain(s).", openID, _dicModels.Count);
				//}
			}
			else
			{
				_dicModels.TryAdd(openID, new WeChatCustomerServiceModel(openID));

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	Utils.DBLogger.Instance.InfoFormat("Use {0} join into conversation list, {1} user(s) remain(s).", openID, _dicModels.Count);
				//}
			}
		}

		public bool IsExist(string openID)
		{
			return _dicModels.ContainsKey(openID);
		}
		#endregion
	}

	#region "Class: WeChatCustomerServiceModel"
	[Serializable]
	internal class WeChatCustomerServiceModel
	{
		#region "Constructs"
		public WeChatCustomerServiceModel(string openID)
		{
			this.OpenID = openID;
			this.CreateDate = DateTime.Now;
			this.ExpireDate = this.CreateDate.Value.AddMinutes(10);
		}
		#endregion

		#region "Properties"
		[Newtonsoft.Json.JsonProperty("openID")]
		public string OpenID { get; set; }

		[Newtonsoft.Json.JsonProperty("createDate")]
		public DateTime? CreateDate { get; private set; }

		[Newtonsoft.Json.JsonProperty("expireDate")]
		public DateTime? ExpireDate { get; private set; }
		#endregion

		#region "Methods"
		public void Refresh()
		{
			if (this.ExpireDate.HasValue && TimeSpan.FromTicks(Math.Abs(DateTime.Now.Ticks - this.ExpireDate.Value.Ticks)).TotalMinutes <= 2)
			{
				this.ExpireDate = DateTime.Now.AddMinutes(10);
			}
		}
		#endregion
	}
	#endregion
}
