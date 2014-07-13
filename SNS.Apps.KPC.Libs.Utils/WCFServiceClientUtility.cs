using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Utils
{
	public static class WCFServiceClientUtility
	{
		#region "Methods: CreateServiceChanel"
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IContract"></typeparam>
		/// <param name="endpointConfigName"></param>
		/// <returns></returns>
		public static IContract CreateServiceChanel<IContract>(string endpointConfigName)
		{
			try
			{
				var channel = (new ChannelFactory<IContract>(endpointConfigName)).CreateChannel();

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	DBLogger.Instance.InfoFormat("Client Service Connection '{0}' has been created!", typeof(IContract).Name);
				//}

				return channel;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("Fail to create service connection: {0}\r\nException: {1}", typeof(IContract).Name, ex.ToString());
			}

			return default(IContract);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IContract"></typeparam>
		/// <typeparam name="TBinding"></typeparam>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static IContract CreateServiceChanel<IContract, TBinding>(Uri uri)
			where TBinding : Binding
		{
			ConstructorInfo cons = typeof(TBinding).GetConstructor(new Type[] { });
			TBinding binding = (TBinding)cons.Invoke(new object[] { });

			return CreateServiceChanel<IContract, TBinding>(binding, new EndpointAddress(uri));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IContract"></typeparam>
		/// <param name="bind"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public static IContract CreateServiceChanel<IContract, TBinding>(TBinding binding, EndpointAddress address)
			where TBinding : Binding
		{
			try
			{
				var channel = ChannelFactory<IContract>.CreateChannel(binding, address);

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	DBLogger.Instance.InfoFormat("Client Service Connection '{0}' has been created!", typeof(IContract).Name);
				//}

				return channel;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("Fail to create service connection: {0}\r\nException: {1}", typeof(IContract).Name, ex.ToString());
			}

			return default(IContract);
		}
		#endregion

		#region "Methods: CloseServiceChannel"
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IContract"></typeparam>
		/// <param name="serviceChannel"></param>
		public static void CloseServiceChannel<IContract>(IContract serviceChannel)
		{
			try
			{
				if (serviceChannel != null)
				{
					ICommunicationObject commu = (serviceChannel as ICommunicationObject);

					if (commu != null && commu.State == CommunicationState.Opened)
					{
						commu.Close();
					}
				}

				//if (Configurations.ConfigStore.CommonSettings.Trace_Mode)
				//{
				//	DBLogger.Instance.InfoFormat("Client Service Connection '{0}' has been closed!", typeof(IContract).Name);
				//}
			}
			catch(Exception ex)
			{
				DBLogger.Instance.ErrorFormat("Fail to close service connection: {0}\r\nException: {1}", typeof(IContract).Name, ex.ToString());
			}
		}
		#endregion
	}
}
