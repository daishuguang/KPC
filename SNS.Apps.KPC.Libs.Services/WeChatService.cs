using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;
using SNS.Apps.KPC.Libs.WeChatHelper;

namespace SNS.Apps.KPC.Libs.Services
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class WeChatService : IWeChatService
	{
		#region "Methods: GetUserInfo"
		public WeChat_UserInfo GetUserInfo(string openID)
		{
			try
			{
				return WeChatHelper.WeChatAPIExecutor.GetUserInfo(openID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"WeChatService",
					"WeChat_UserInfo GetUserInfo(string openID)",
					string.Format("OpenID: {0}", openID),
					ex.ToString()
				);
			}

			return null;
		}

		public WeChat_UserInfo GetUserInfo(string openID, string accessToken, string refreshToken)
		{
			try
			{
				return WeChatHelper.WeChatAPIExecutor.GetAuthUserInfo(openID, accessToken, refreshToken);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"WeChatService",
					"WeChat_UserInfo GetUserInfo(string openID, string accessToken, string refreshToken)",
					string.Format("OpenID: {0}, AccessToken: {1}, RefreshToken: {2}", openID, accessToken, refreshToken),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: GetUserList"
		public WeChat_UserList GetUserList()
		{
			try
			{
				return WeChatHelper.WeChatAPIExecutor.GetUserList();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: []\r\nException: {2}",
					"WeChatService",
					"WeChat_UserList GetUserList()",
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: AuthenticateUser"
		public WeChat_OAuth2_AccessTokenInfo AuthenticateUser(string code, WeChat_OAuth2_AuthScope scope = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo)
		{
			try
			{
				return WeChatHelper.WeChatAPIExecutor.AuthticateUser(code, scope);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"WeChatService",
					"WeChat_OAuth2_AccessTokenInfo AuthenticateUser(string code, WeChat_OAuth2_AuthScope mode = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo)",
					string.Format("Code: {0}, Mode: {1}", code, scope),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: SendMessage"
		public bool SendMessage(long userID, string msg, bool enableSMS = true)
		{
			var userInstance = UserRepository.Instance.Get(userID);

			if (userInstance != null)
			{
				return SendMessage(userInstance.OpenID, msg, enableSMS);
			}

			return false;
		}

		public bool SendMessage(Guid userID, string msg, bool enableSMS = true)
		{
			var userInstance = UserRepository.Instance.Get(userID);

			if (userInstance != null)
			{
				return SendMessage(userInstance.OpenID, msg, enableSMS);
			}

			return false;
		}

		public bool SendMessage(string openID, string msg, bool enableSMS = true)
		{
			try
			{
				var flag = false;

				if (!string.IsNullOrEmpty(openID))
				{
					// 首先尝试调用微信客服API向用户推送微信消息
					flag = WeChatAPIExecutor.SendMessage(openID, msg);
				}

				if (flag)
				{
					return true;
				}

				if (enableSMS && !flag)
				{
					// 否则调用SMSService向用户发送短信
					var userInstance = UserRepository.Instance.Get(openID);

					if (userInstance != null && !string.IsNullOrEmpty(userInstance.Mobile))
					{
						var clientSMS = default(ISMSService);

						try
						{
							clientSMS = Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<ISMSService>("smsService");

							var result = clientSMS.SendSMS_Notification(new Models.SMS.SMSMessageSendRequest { Channel = 0, Mobiles = new string[] { userInstance.Mobile }, Content = msg });

							flag = (result != null && result.Success);
						}
						catch (Exception)
						{
							flag = false;
						}
						finally
						{
							Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(clientSMS);
						}

						return flag;
					}
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"WeChatService",
					"bool SendMessage(string openID, string msg)",
					string.Format("OpenID: {0}, Msg: {1}", openID, msg),
					ex.ToString()
				);
			}

			return false;
		}

		public bool SendMessage(CustomerMessageBase msg)
		{
			try
			{
				return WeChatAPIExecutor.SendMessage(msg);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"WeChatService",
					"bool SendMessage(CustomerMessageBase<MessageContentBase> msg)",
					string.Format("Msg: {0}", msg),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion
	}
}
