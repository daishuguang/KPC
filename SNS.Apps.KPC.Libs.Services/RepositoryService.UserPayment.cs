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
		#region "Methods: ExecutePayment"
		public bool ExecutePayment(PaymentSubmitRequest requestInfo)
		{
			try
			{
				return UserPaymentRepository.Instance.Execute(requestInfo);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool ExecutePayment(PaymentSubmitRequest requestInfo)",
					requestInfo.ToString(),
					ex.ToString()
				);
			}

			return false;
		} 
		#endregion

		#region "Methods: FinishPayment"
		public bool FinishPayment(long orderID, string payFolio)
		{
			try
			{
				return UserPaymentRepository.Instance.Finish(orderID, payFolio);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool FinishPayment(long orderID, string payFolio)",
					string.Format("OrderID: {0}, PayFolio: {1}", orderID, payFolio),
					ex.ToString()
				);
			}

			return false;
		} 
		#endregion

		#region "Methods: ValidatePayment"
		public bool ValidatePayment(long orderID, string payFolio)
		{
			try
			{
				return UserPaymentRepository.Instance.Validate(orderID, payFolio);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool ValidatePayment(long orderID, string payFolio)",
					string.Format("OrderID: {0}, PayFolio: {1}", orderID, payFolio),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion

		#region "Methods: GetPayment"
		public PaymentSubmitResult GetPayment(long orderID)
		{
			try
			{
				return UserPaymentRepository.Instance.Get(orderID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"PaymentSubmitResult GetPayment(long orderID)",
					string.Format("OrderID: {0}", orderID),
					ex.ToString()
				);
			}

			return null;
		}

		public PaymentSubmitResult GetPayment(string orderID)
		{
			try
			{
				return UserPaymentRepository.Instance.Get(orderID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"PaymentSubmitResult GetPayment(string orderID)",
					string.Format("OrderID: {0}", orderID),
					ex.ToString()
				);
			}

			return null;
		} 
		#endregion
	}
}
