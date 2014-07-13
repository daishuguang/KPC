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
		#region "Methods: GetRoute"
		[OperationContract(Name = "GetRouteByRouteID")]
		Route GetRoute(long routeID, bool inForce = false);

		[OperationContract(Name = "GetRouteByRouteGUID")]
		Route GetRoute(Guid routeID, bool inForce = false);
		#endregion

		#region "Methods: SetRouteInavailable"
		[OperationContract(Name = "DeleteRouteByRouteID")]
		void DeleteRoute(long routeID);

		[OperationContract(Name = "DeleteRouteByRouteGUID")]
		void DeleteRoute(Guid routeID); 
		#endregion
	}
}
