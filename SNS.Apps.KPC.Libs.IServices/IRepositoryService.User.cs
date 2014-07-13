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
	public partial interface IRepositoryService
	{
		#region "Methods: GetUser"
		[OperationContract(Name = "GetUserByUserID")]
		User GetUser(long userID, bool inForce = false);

		[OperationContract(Name = "GetUserByUserGUID")]
		User GetUser(Guid userID, bool inForce = false);

		[OperationContract(Name = "GetUserByOpenID")]
		User GetUser(string openID, bool inForce = false);

		[OperationContract(Name = "GetUserByMobile")]
		User GetUserWithMobile(string mobile, bool inForce = false);

		[OperationContract(Name = "GetUserByMobileSec")]
		User GetUserWithMobile(string mobile, string password);
		#endregion

		#region "Methods: ResetUserPass"
		[OperationContract]
		bool ResetUserPass(string mobile, string password); 
		#endregion

		#region "Methods: CheckIsFirst"
		[OperationContract]
		bool CheckIsSubscribeFirst(long userID);

		[OperationContract]
		bool CheckIsRegisterFirst(long userID);

		[OperationContract]
		bool CheckIsUnsubscribeFirst(long userID);

		[OperationContract]
		UserPrivy GetUserPrivy(long id);

		[OperationContract]
		UserSecurity GetUserSecurity(long id);
		#endregion

		#region "Methods: 关注 & 取消关注"
		[OperationContract]
		User Subscribe(string openID, WeChat_UserInfo userInfo, int? refID = null);

		[OperationContract]
		void Unsubscribe(string openID);
		#endregion

		#region "Methods: Register"
		[OperationContract]
		User Register(UserRegisterModel registerInfo); 
		#endregion

		#region "Methods: EditProfile"
		[OperationContract]
		User EditProfile(UserEditModel editInfo); 
		#endregion

		#region "Methods: UpdateProfile"
		[OperationContract]
		void UpdateProfile(long userID, string openID, WeChat_UserInfo userInfo);
		#endregion
	}
}
