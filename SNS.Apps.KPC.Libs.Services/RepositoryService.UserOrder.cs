using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Services
{
	public partial class RepositoryService
	{
		#region "Methods: GetUserOrder"
		public UserOrderResult GetUserOrder(string folio, bool inForce = false)
		{
			try
			{
				var uo = UserOrderRepository.Instance.Get(folio, inForce);

				return (uo != null) ? (new UserOrderResult(uo)) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserOrderResult GetUserOrder(string folio, bool inForce = false)",
					string.Format("Folio: {0}, InForce: {1}", folio, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public IEnumerable<UserOrderResult> GetUserOrder(UserOrderRequest requestInfo, bool inForce = false)
		{
			try
			{
				var lst = new List<UserOrderResult>();
				var uos = UserOrderRepository.Instance.Get(requestInfo);

				if (uos != null)
				{
					foreach (var uo in uos)
					{
						lst.Add(new UserOrderResult(uo));
					}
				}

				return lst.ToArray();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"IEnumerable<UserOrderResult> GetUserOrder(UserOrderRequest requestInfo, bool inForce = false)",
					string.Format("UserOrderRequest: {0}, InForce: {1}", requestInfo.ToString(), inForce),
					ex.ToString()
				);
			}

			return (new UserOrderResult[] { });
		}
		#endregion

		#region "Methods: CreateUserOrder"
		public string CreateUserOrder(UserOrderCreateRequest requestInfo)
		{
			try
			{
				return UserOrderRepository.Instance.Create(requestInfo);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"string CreateUserOrder(OrderCreateRequest requestInfo)",
					requestInfo.ToString(),
					ex.ToString()
				);
			}

			return string.Empty;
		}
		#endregion

		#region "Methods: UpdateUserOrder"
		public bool UpdateUserOrder(UserOrderUpdateRequest requestInfo)
		{
			try
			{
				return UserOrderRepository.Instance.Update(requestInfo);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"boo UpdateUserOrder(UserOrderUpdateRequest requestInfo)",
					requestInfo.ToString(),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion

		#region "Methods: CancelUserOrder"
		public bool CancelUserOrder(Guid userID, string folio)
		{
			try
			{
				return UserOrderRepository.Instance.Cance(userID, folio);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool CancelUserOrder(Guid userID, string folio)",
					string.Format("UserID: {0}, Folio: {1}", folio, userID),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion

		#region "Methods: ConfirmUserOrder"
		public bool ConfirmUserOrder(Guid userID, string folio)
		{
			try
			{
				return UserOrderRepository.Instance.Confirm(userID, folio);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"bool ConfirmUserOrder(Guid userID, string folio)",
					string.Format("UserID: {0}, Folio: {1}", folio, userID),
					ex.ToString()
				);
			}

			return false;
		}
		#endregion
	}
}
