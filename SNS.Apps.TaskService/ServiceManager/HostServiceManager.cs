using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.TaskService
{
	class HostServiceManager
	{
		#region "Fields"
		static Lazy<IEnumerable<ServiceHost>> _services = new Lazy<IEnumerable<ServiceHost>>(() =>
		{
			var lst = new List<ServiceHost>();
			var services = ConfigurationManager.GetSection("hostServiceSettings") as HostServiceConfiguration;

			if (services != null)
			{
				foreach (HostServiceElement service in services.Services)
				{
					try
					{
						var instance = new ServiceHost(Type.GetType(service.Type));

						lst.Add(instance);
					}
					catch(Exception ex)
					{
						DBLogger.Instance.Error(ex.ToString());
					}
				}
			}

			return lst.ToArray();
		});
		#endregion

		#region "Constructs"
		/// <summary>
		/// 
		/// </summary>		
		private HostServiceManager() { }
		#endregion

		#region "Properties"
		public static bool IsRunning { get; private set; }

		public static IEnumerable<ServiceHost> Services { get { return _services.Value; } }
		#endregion

		#region "Events"
		static void host_Opened(object sender, EventArgs e)
		{
			DBLogger.Instance.InfoFormat("Host Service '{0}' is running....", (sender as ServiceHost).Description.ServiceType.FullName);
		}

		static void host_Closed(object sender, EventArgs e)
		{
			DBLogger.Instance.InfoFormat("Host Service '{0}' is closed.", (sender as ServiceHost).Description.ServiceType.FullName);
		}

		static void host_Faulted(object sender, EventArgs e)
		{
			DBLogger.Instance.InfoFormat("Host Service '{0}' is faulted.", (sender as ServiceHost).Description.ServiceType.FullName);
		}

		static void host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			DBLogger.Instance.InfoFormat("Host Service '{0}' get the Unknow Message.", (sender as ServiceHost).Description.ServiceType.FullName);
		}
		#endregion

		#region "Public Methods"
		/// <summary>
		/// 
		/// </summary>
		public static void Start()
		{
			if (IsRunning || Services == null || Services.Count() == 0)
			{
				return;
			}

			foreach (var host in Services)
			{
				try
				{
					host.Opened += host_Opened;
					host.Closed += host_Closed;
					host.Faulted += host_Faulted;
					host.UnknownMessageReceived += host_UnknownMessageReceived;

					host.Open(TimeSpan.FromSeconds(2));
				}
				catch (Exception ex)
				{
					DBLogger.Instance.Error(ex.ToString());
				}
			}

			IsRunning = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Stop()
		{
			if (Services == null || Services.Count() == 0)
			{
				return;
			}

			foreach (var s in Services)
			{
				if (s != null && s.State != CommunicationState.Closed)
				{
					try
					{
						s.Close();
					}
					catch (Exception ex)
					{
						DBLogger.Instance.Error(ex.ToString());
					}
				}
			}

			IsRunning = false;
		}
		#endregion
	}
}
