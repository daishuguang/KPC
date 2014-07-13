using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Libs.IServices
{
	public partial interface IRepositoryService
	{
		#region "GetUserOrder"
		[OperationContract(Name = "GetUserOrderByFolio")]
		UserOrderResult GetUserOrder(string folio, bool inForce = false);

		[OperationContract(Name = "GetUserOrderByFilter")]
		IEnumerable<UserOrderResult> GetUserOrder(UserOrderRequest requestInfo, bool inForce = false);
		#endregion

		[OperationContract]
		string CreateUserOrder(UserOrderCreateRequest requestInfo);

		[OperationContract]
		bool UpdateUserOrder(UserOrderUpdateRequest requestInfo);

		[OperationContract]
		bool CancelUserOrder(Guid userID, string folio);

		[OperationContract]
		bool ConfirmUserOrder(Guid userID, string folio);
	}
}
