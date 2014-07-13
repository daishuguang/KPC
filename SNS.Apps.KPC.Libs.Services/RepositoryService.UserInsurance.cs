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
	public partial class RepositoryService : IRepositoryService
	{
		#region "Methods: GetInsuranceOrder"
		public UserInsuranceResult GetInsuranceOrder(string folio, bool inForce = false)
		{
			try
			{
				var result = UserInsuranceRepository.Instance.Get(folio, inForce);

				return (result != null) ? (new UserInsuranceResult(result)) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserInsuranceResult GetOrder(string folio, bool inForce = false)",
					string.Format("Folio: {0}, InForce: {1}", folio, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public UserInsuranceResult GetInsuranceOrder(long id, bool inForce = false)
		{
			try
			{
				var result = UserInsuranceRepository.Instance.Get(id, inForce);

				return (result != null) ? (new UserInsuranceResult(result)) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserInsuranceResult GetOrder(long id, bool inForce = false)",
					string.Format("ID: {0}, InForce: {1}", id, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public UserInsuranceResult GetInsuranceOrder(long userID, long orderID, bool inForce = false)
		{
			try
			{
				var result = UserInsuranceRepository.Instance.Get(userID, orderID, inForce);

				return (result != null) ? (new UserInsuranceResult(result)) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserInsuranceResult GetOrder(long userID, long orderID, bool inForce = false)",
					string.Format("UserID: {0}, OrderID: {1}, InForce: {2}", userID, orderID, inForce),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: GetInsuranceOrderLatest"
		public UserInsuranceResult GetInsuranceOrderLatest(long userID, bool includeInActive = false, bool inForce = false)
		{
			try
			{
				var result = UserInsuranceRepository.Instance.GetLatest(userID, includeInActive, inForce);

				return (result != null) ? (new UserInsuranceResult(result)) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserInsuranceResult GetInsuranceOrderLatest(long userID, bool includeInActive = false, bool inForce = false)",
					string.Format("UserID: {0}, IncludeInActive: {1}, InForce: {2}", userID, includeInActive, inForce),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion
	}
}
