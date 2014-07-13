using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
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
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class RouteMatrixService : IRouteMatrixService
	{
		public Guid Publish(RouteCreateRequest requestInfo)
		{
			try
			{
				var t1 = DateTime.Now.Ticks;

				requestInfo.Route.RouteGUID = RouteMatrixRepository.Instance.Publish(requestInfo);

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat(
						"Success to publish new Route: {0}, with Parameters: [{1}] in [{2}] second(s)",
						requestInfo.Route.RouteGUID,
						string.Format("UserID: {0}, UserRole: {1}, From: {2}, To: {3}", requestInfo.User.UserGUID, requestInfo.UserRole, (!string.IsNullOrEmpty(requestInfo.Route.To_Location)) ? (requestInfo.Route.From_City + requestInfo.Route.From_District) : (requestInfo.Route.From_City), (!string.IsNullOrEmpty(requestInfo.Route.To_Location)) ? (requestInfo.Route.To_City + requestInfo.Route.To_District) : (requestInfo.Route.To_City)),
						TimeSpan.FromTicks(DateTime.Now.Ticks - t1).TotalSeconds
					);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RouteMatrixService",
					"public bool Publish(RouteCreateModel requestInfo)",
					string.Format("UserID: {0}, UserRole: {1}, From: {2}, To: {3}", requestInfo.User.UserGUID, requestInfo.UserRole, (!string.IsNullOrEmpty(requestInfo.Route.To_Location)) ? (requestInfo.Route.From_City + requestInfo.Route.From_District) : (requestInfo.Route.From_City), (!string.IsNullOrEmpty(requestInfo.Route.To_Location)) ? (requestInfo.Route.To_City + requestInfo.Route.To_District) : (requestInfo.Route.To_City)),
					ex.ToString()
				);
			}

			return requestInfo.Route.RouteGUID;
		}

		public IEnumerable<RouteSearchResult> Search(RouteSearchRequest requestInfo)
		{
			try
			{
				var t1 = DateTime.Now.Ticks;
				var results = RouteMatrixRepository.Instance.Search(requestInfo.Filter, requestInfo.Page, requestInfo.Count);

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat(
						"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nResults: Return [{3}] record(s) in [{4}] second(s)",
						"RouteMatrixService",
						"IEnumerable<RouteSearchResult> Search(RouteSearchRequest requestInfo)",
						string.Format("RequestInfo: {0}", requestInfo.ToString()),
						(results != null) ? (results.Count()) : (0),
						TimeSpan.FromTicks(DateTime.Now.Ticks - t1).TotalSeconds
					);
				}

				return results;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RouteMatrixService",
					"IEnumerable<RouteSearchResult> Search(RouteSearchRequest requestInfo)",
					string.Format("RequestInfo: {0}", requestInfo.ToString()),
					ex.ToString()
				);
			}

			return new RouteSearchResult[] { };
		}
	}
}
