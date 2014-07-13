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
		#region "GetUserRouteByRouteID"
		[OperationContract(Name = "GetUserRouteByRouteID")]
		UserRouteResult GetUserRouteByRouteID(long routeID, bool inForce = false);

		[OperationContract(Name = "GetUserRouteByRouteGUID")]
		UserRouteResult GetUserRouteByRouteID(Guid routeID, bool inForce = false);
		#endregion

		#region "GetUserRouteByUserID"
		[OperationContract(Name = "LoadUserRouteByUserID")]
		IEnumerable<UserRouteResult> LoadUserRouteByUserID(long userID, int page = 0, int count = 10, bool inForce = false);

		[OperationContract(Name = "LoadUserRouteByUserGUID")]
		IEnumerable<UserRouteResult> LoadUserRouteByUserID(Guid userID, int page = 0, int count = 10, bool inForce = false);

		[OperationContract(Name = "LoadUserRouteByOpenID")]
		IEnumerable<UserRouteResult> LoadUserRouteByUserID(string openID, int page = 0, int count = 10, bool inForce = false);
		#endregion

		#region "GetUserRouteLatest"
		[OperationContract(Name = "GetUserRouteNewestByUserID")]
		UserRouteResult GetUserRouteNewest(long userID, bool inForce = false);

		[OperationContract(Name = "GetUserRouteNewestByUserGUID")]
		UserRouteResult GetUserRouteNewest(Guid userID, bool inForce = false);

		[OperationContract(Name = "GetUserRouteNewestByOpenID")]
		UserRouteResult GetUserRouteNewest(string openID, bool inForce = false);
		#endregion

		#region "LoadUserRouteNewest"
		[OperationContract]
		IEnumerable<UserRouteResult> LoadUserRouteNewest(RouteSearchRequest requestInfo, bool inForce = false);

		[OperationContract]
		IEnumerable<UserRouteResult> LoadUserRouteNewest4Disp(string city, bool inForce = false);
		#endregion
	}
}
