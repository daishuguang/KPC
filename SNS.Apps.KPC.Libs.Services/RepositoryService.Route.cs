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
		#region "Methods: GetRoute"
		public Route GetRoute(long routeID, bool inForce = false)
		{
			try
			{
				return RouteRepository.Instance.Get(routeID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [RouteID: {2}, InForce: {3}]\r\nException: {4}",
					"RepositoryService",
					"public Route GetRoute(long routeID, bool inForce = false)",
					routeID,
					inForce,
					ex.ToString()
				);
			}

			return null;
		}

		public Route GetRoute(Guid routeID, bool inForce = false)
		{
			try
			{
				return RouteRepository.Instance.Get(routeID, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [RouteID: {2}, InForce: {3}]\r\nException: {4}",
					"RepositoryService",
					"public Route GetRoute(Guid routeID, bool inForce = false)",
					routeID,
					inForce,
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: DeleteRoute"
		public void DeleteRoute(long routeID)
		{
			try
			{
				RouteRepository.Instance.SetStatus(routeID, RouteStatus.Inavailable);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [RouteID: {2}]\r\nException: {3}",
					"RepositoryService",
					"void DeleteRoute(long routeID)",
					routeID,
					ex.ToString()
				);
			}
		}

		public void DeleteRoute(Guid routeID)
		{
			try
			{
				RouteRepository.Instance.SetStatus(routeID, RouteStatus.Inavailable);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [RouteID: {2}]\r\nException: {3}",
					"RepositoryService",
					"void DeleteRoute(Guid routeID)",
					routeID,
					ex.ToString()
				);
			}
		} 
		#endregion
	}
}
