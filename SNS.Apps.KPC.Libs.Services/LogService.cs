using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Policy;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Services
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class LogService : ILogService
	{
		#region "Methods: WXMessage"
		public WXMessage GetLatestWXMsg(long userID, bool inForce = false)
		{
			try
			{
				return LogRepository.Instance.GetLatestWXMsg(userID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"WXMessage GetLatestWXMsg(long userID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", userID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public WXMessage GetLatestWXMsg(string openID, bool inForce = false)
		{
			try
			{
				return LogRepository.Instance.GetLatestWXMsg(openID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"WXMessage GetLatestWXMsg(long openID, bool inForce = false)",
					string.Format("OpenID: {0}, InForce: {1}", openID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public void ArchiveWXMsg(WXMessage msg)
		{
			try
			{
				LogRepository.Instance.ArchiveWXMsg(msg);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"void ArchiveWXMsg(WXMessage msg)",
					string.Format("MSG: {0}", msg.ToString()),
					ex.ToString()
				);
			}
		}
		#endregion
	}
}
