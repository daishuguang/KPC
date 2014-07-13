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
		#region "UserTrack Service"

		#region "GetUserTrack"
		[OperationContract(Name = "GetUserTrackByUserID")]
		UserTrackResult GetUserTrack(long userID, bool inForce = false);

		[OperationContract(Name = "GetUserTrackByUserGUID")]
		UserTrackResult GetUserTrack(Guid userID, bool inForce = false);

		[OperationContract(Name = "GetUserTrackByOpenID")]
		UserTrackResult GetUserTrack(string openID, bool inForce = false);
		#endregion

		#region "SetUserTrack"
		[OperationContract(Name = "SetUserTrackByUserID")]
		void SetUserTrack(long userID, Point position);

		[OperationContract(Name = "SetUserTrackByUserGUID")]
		void SetUserTrack(Guid userID, Point position);

		[OperationContract(Name = "SetUserTrackByOpenID")]
		void SetUserTrack(string openID, Point position);
		#endregion

		#region "LoadUserAround"
		[OperationContract(Name = "LoadUserAroundList")]
		IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestList requestInfo);

		[OperationContract(Name = "LoadUserAroundMap")]
		IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestMap requestInfo);
		#endregion

		#endregion
	}
}
