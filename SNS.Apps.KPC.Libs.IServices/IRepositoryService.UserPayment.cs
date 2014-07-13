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
		[OperationContract]
		bool ExecutePayment(PaymentSubmitRequest requestInfo);

		[OperationContract]
		bool FinishPayment(long orderID, string payFolio);

		[OperationContract]
		bool ValidatePayment(long orderID, string payFolio);

		[OperationContract(Name = "GetPaymentByID")]
		PaymentSubmitResult GetPayment(long orderID);

		[OperationContract(Name = "GetPaymentByFolio")]
		PaymentSubmitResult GetPayment(string orderID);
	}
}
