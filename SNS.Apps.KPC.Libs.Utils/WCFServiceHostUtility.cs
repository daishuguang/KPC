using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Utils
{
	public static class WCFServiceHostUtility
	{
		#region "Fields"
		static List<ServiceHost> _lstSvcs = new List<ServiceHost>();
		#endregion

		#region "Properties"
		public static ServiceHost[] Services
		{
			get { return _lstSvcs.ToArray(); }
		}

		public static bool IsRunning { get; set; }
		#endregion

		#region "Public Methods"
		/// <summary>
		/// 
		/// </summary>
		public static void Start()
		{
			if (_lstSvcs != null && _lstSvcs.Count > 0)
			{
				_lstSvcs.ForEach(host =>
				{
					host.Opened += new EventHandler(ServiceHost_Opened);
					host.Faulted += new EventHandler(ServiceHost_Faulted);
					host.Closed += new EventHandler(ServiceHost_Closed);
					host.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(ServiceHost_UnknownMessageReceived);

					try 
					{ 
						host.Open(TimeSpan.FromSeconds(1)); 
					}
					catch (Exception ex) 
					{
						DBLogger.Instance.Error(ex.ToString());
					}
				});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Stop()
		{
			if (_lstSvcs != null && _lstSvcs.Count > 0)
			{
				_lstSvcs.ForEach(host => 
				{
					if (host != null && host.State != CommunicationState.Closed)
					{
						host.Close();						
					}
				});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t"></param>
		/// <param name="baseAddrs"></param>
		public static void AddService(Type t, params Uri[] baseAddrs)
		{
			if (!_lstSvcs.Exists(host => host.GetType().Equals(t)))
			{
				_lstSvcs.Add(new ServiceHost(t, baseAddrs));
			}
		}
		#endregion

		#region "Events"
		static void ServiceHost_Opened(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		static void ServiceHost_Closed(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		static void ServiceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		static void ServiceHost_Faulted(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}
		#endregion
	}
}
