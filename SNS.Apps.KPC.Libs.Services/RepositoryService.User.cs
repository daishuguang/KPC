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
	public partial class RepositoryService
	{
		#region "Public Methods"

		#region "Methods: GetUser"
		public User GetUser(long userID, bool inForce = false)
		{
			try
			{
				return UserRepository.Instance.Get(userID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User GetUser(long userID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", userID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public User GetUser(Guid userID, bool inForce = false)
		{
			try
			{
				return UserRepository.Instance.Get(userID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User GetUser(Guid userID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", userID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public User GetUser(string openID, bool inForce = false)
		{
			try
			{
				return UserRepository.Instance.Get(openID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User GetUser(string openID, bool inForce = false)",
					string.Format("OpenID: {0}, InForce: {1}", openID, inForce),
					ex.ToString()
				);
			}

			return null;
		} 

		public User GetUserWithMobile(string mobile, bool inForce = false)
		{
			try
			{
				return UserRepository.Instance.GetWithMobile(mobile, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User GetUserWithMobile(string mobile, bool inForce = false)",
					string.Format("Mobile: {0}, InForce: {1}", mobile, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public User GetUserWithMobile(string mobile, string password)
		{
			try
			{
				return UserRepository.Instance.GetWithMobile(mobile, password);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User GetUserWithMobile(string mobile, string password, bool inForce = false)",
					string.Format("Mobile: {0}, Password: {1}", mobile, password),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPrivy GetUserPrivy(long id)
		{
			try
			{
				return UserRepository.Instance.GetPrivy(id);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPrivy GetUserPrivy(long id)",
					string.Format("ID: {0}", id),
					ex.ToString()
				);
			}

			return null;
		}

		public UserSecurity GetUserSecurity(long id)
		{
			try
			{
				return UserRepository.Instance.GetSecurity(id);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserSecurity GetUserSecurity(long id)",
					string.Format("ID: {0}", id),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: ResetUserPass"
		public bool ResetUserPass(string mobile, string password)
		{
			try
			{
				return UserRepository.Instance.ResetPass(mobile, password);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool ResetUserPass(string mobile, string password)",
					string.Format("Mobile: {0}, Password: {1}", mobile, password),
					ex.ToString()
				);
			}

			return false;
		} 
		#endregion

		#region "Methods: CheckIsRegisterFirst"
		public bool CheckIsSubscribeFirst(long userID)
		{
			try
			{
				return UserRepository.Instance.CheckIsSubscribeFirst(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool CheckIsSubscribeFirst(long userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return false;
		}

		public bool CheckIsRegisterFirst(long userID)
		{
			try
			{
				return UserRepository.Instance.CheckIsRegisterFirst(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool CheckIsRegisterFirst(long userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return false;
		}

		public bool CheckIsUnsubscribeFirst(long userID)
		{
			try
			{
				return UserRepository.Instance.CheckIsUnsubscribeFirst(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool CheckIsUnsubscribeFirst(long userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion

		#region "Methods: 关注 & 取消关注 & 更新"
		public User Subscribe(string openID, WeChat_UserInfo userWeChat, int? refID = null)
		{
			try
			{
				var userInstance = UserRepository.Instance.Subscribe(openID, userWeChat, refID);

				if (userInstance != null)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateProfile), new { UserID = userInstance.ID, OpenID = userInstance.OpenID, UserWeChat = userWeChat });
				}

				return userInstance;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"User Subscribe(string openID, WeChat_UserInfo userInfo, int? refID = null)",
					string.Format("OpenID: {0}, User: {1}, RefID: {2}", openID, (userWeChat != null ? userWeChat.ToString() : string.Empty), (refID != null && refID.HasValue ? refID.Value.ToString() : string.Empty)),
					ex.ToString()
				);
			}

			return null;
		}

		public void Unsubscribe(string openID)
		{
			try
			{
				UserRepository.Instance.UnSubscribe(openID);
			}
			catch(Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"void Unsubscribe(string openID)",
					string.Format("OpenID: {0}", openID),
					ex.ToString()
				);
			}
		} 
		#endregion

		#region "Methods: 注册 & 编辑"
		public User Register(UserRegisterModel registerInfo)
		{
			try
			{
				return UserRepository.Instance.Register(registerInfo);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [UserGUID: {2}, NickName: {3}]\r\nException: {4}",
					"RepositoryService",
					"public User Register(UserRegisterModel registerInfo)",
					registerInfo.UserGUID,
					registerInfo.NickName,
					ex.ToString()
				);
			}

			return null;
		}

		public User EditProfile(UserEditModel editInfo)
		{
			try
			{
				return UserRepository.Instance.EditProfile(editInfo);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [UserGUID: {2}, NickName: {3}]\r\nException: {4}",
					"RepositoryService",
					"User EditProfile(UserEditModel editInfo)",
					editInfo.UserGUID,
					editInfo.NickName,
					ex.ToString()
				);
			}

			return null;
		} 
		#endregion

		#region "Methods: UpdateUserProfile"
		public void UpdateProfile(long userID, string openID, WeChat_UserInfo userWeChat)
		{
			try
			{
				if (userWeChat == null)
				{
					userWeChat = WeChatHelper.WeChatAPIExecutor.GetUserInfo(openID);
				}

				if (userWeChat != null)
				{
					UserRepository.Instance.UpdateProfile(userID, userWeChat);

					if (ConfigStore.CommonSettings.Trace_Mode)
					{
						DBLogger.Instance.InfoFormat("Success to update UserProfile for user: UserID: {0}, OpenID: {1}", userID, openID);
					}
				}
				else
				{
					throw new Exception(string.Format("Cannot get UserProfile data through the API for user: UserID: {0}, OpenID: {1}", userID, openID));
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"void UpdateProfile(long userID, string openID, WeChat_UserInfo userWeChat)",
					string.Format("UserID: {0}, OpenID: {1}, UserWeChat: {2}", userID, openID, (userWeChat != null ? userWeChat.ToString() : string.Empty)),
					ex.ToString()
				);
			}
		}

		void UpdateProfile(dynamic state)
		{
			try
			{
				if (state == null)
				{
					throw new Exception("Fail to update UserProfile for user with exception: The parameter 'state' should be null!");
				}

				UpdateProfile(state.UserID, state.OpenID, state.UserWeChat);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("Fail to call method 'void UpdateProfile(long userID, string openID, WeChat_UserInfo userWeChat)' for updating UserProfile with exception: {0}", ex.ToString());
			}
		}
		#endregion
		#endregion
	}
}
