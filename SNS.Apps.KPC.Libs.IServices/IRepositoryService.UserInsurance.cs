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
		#region "Methods: GetInsuranceOrder"
		[OperationContract(Name = "GetInsuranceOrderByFolio")]
		UserInsuranceResult GetInsuranceOrder(string folio, bool inForce = false);

		[OperationContract(Name = "GetInsuranceOrderByID")]
		UserInsuranceResult GetInsuranceOrder(long id, bool inForce = false);

		[OperationContract(Name = "GetInsuranceOrderByOrderID")]
		UserInsuranceResult GetInsuranceOrder(long userID, long orderID, bool inForce = false); 
		#endregion

		#region "Methods: GetInsuranceOrderLatest"
		[OperationContract]
		UserInsuranceResult GetInsuranceOrderLatest(long userID, bool includeInActive = false, bool inForce = false); 
		#endregion
	}
}
