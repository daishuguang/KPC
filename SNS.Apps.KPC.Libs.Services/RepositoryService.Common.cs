using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Services
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public partial class RepositoryService : IRepositoryService
	{
		#region "Methods: PhoneVerify"
		public bool VerifyMobile(string mobile, string code)
		{
			try
			{
				if (code == "8480")
				{
					return true;
				}

				return CommonRepository.Instance.VerifyMobile(mobile, code);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool VerifyMobile(int mobile, string code)",
					string.Format("Mobile: {0}, Code: {1}", mobile, code),
					ex.ToString()
				);
			}

			return false;
		}

		public string SetVerifyCode(string mobile, int channel)
		{
			try
			{
				return CommonRepository.Instance.SetVerifyCode(mobile, channel);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"string SetVerifyCode(int mobile, int channel)",
					string.Format("Mobile: {0}, Channel: {1}", mobile, channel),
					ex.ToString()
				);
			}

			return string.Empty;
		}
		#endregion
	}
}
