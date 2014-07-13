using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;

namespace SNS.Apps.KPC.Libs.IServices
{
	[ServiceContract(Namespace = "api.kuaipinche.com")]
	public interface IWeChatService
	{
		#region "Methods: GetUserInfo"
		[OperationContract]
		WeChat_UserInfo GetUserInfo(string openID);

		[OperationContract(Name = "GetUserInfoWithOAuthToken")]
		WeChat_UserInfo GetUserInfo(string openID, string accessToken, string refreshToken); 
		#endregion

		#region "Methods: GetUserList"
		[OperationContract]
		WeChat_UserList GetUserList(); 
		#endregion

		#region "Methods: AuthenticateUser"
		[OperationContract]
		WeChat_OAuth2_AccessTokenInfo AuthenticateUser(string code, WeChat_OAuth2_AuthScope scope = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo); 
		#endregion

		#region "Methods: SendMessage"
		[OperationContract(Name = "SendMessageByUserID")]
		bool SendMessage(long userID, string msg, bool enableSMS = true);

		[OperationContract(Name = "SendMessageByUserGUID")]
		bool SendMessage(Guid userID, string msg, bool enableSMS = true);

		[OperationContract(Name = "SendMessageByOpenID")]
		bool SendMessage(string openID, string msg, bool enableSMS = true);

		[OperationContract(Name = "SendMessage")]
		bool SendMessage(CustomerMessageBase msg);
		#endregion
	}
}
