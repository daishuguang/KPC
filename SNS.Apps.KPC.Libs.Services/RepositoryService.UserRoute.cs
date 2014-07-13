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
		#region "Methods: GetUserRouteByRouteID"
		public UserRouteResult GetUserRouteByRouteID(long routeID, bool inForce = false)
		{
			var ur = UserRouteRepository.Instance.GetByRouteID(routeID, inForce);

			return (ur != null) ? (new UserRouteResult(ur)) : (null);
		}

		public UserRouteResult GetUserRouteByRouteID(Guid routeID, bool inForce = false)
		{
			var ur = UserRouteRepository.Instance.GetByRouteID(routeID, inForce);

			return (ur != null) ? (new UserRouteResult(ur)) : (null);
		}
		#endregion

		#region "Methods: GetUserRouteByUserID"
		public IEnumerable<UserRouteResult> LoadUserRouteByUserID(long userID, int page = 0, int count = 10, bool inForce = false)
		{
			try
			{
				var lst = new List<UserRouteResult>();
				var urs = UserRouteRepository.Instance.LoadByUserID(userID, page, count, inForce);

				if (urs != null && urs.Count() > 0)
				{
					foreach (var ur in urs)
					{
						lst.Add(new UserRouteResult(ur));
					}
				}

				return lst.ToArray();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}, Page: {3}, Count: {4}\r\nException: {5}",
					"RepositoryService",
					"public IEnumerable<UserRouteResult> LoadUserRouteByUserID(long userID, int page = 0, int count = 10)",
					userID,
					page,
					count,
					ex.ToString()
				);
			}

			return new UserRouteResult[] { };
		}

		public IEnumerable<UserRouteResult> LoadUserRouteByUserID(Guid userID, int page = 0, int count = 10, bool inForce = false)
		{
			try
			{
				var lst = new List<UserRouteResult>();
				var urs = UserRouteRepository.Instance.LoadByUserID(userID, page, count, inForce);

				if (urs != null && urs.Count() > 0)
				{
					foreach (var ur in urs)
					{
						lst.Add(new UserRouteResult(ur));
					}
				}

				return lst.ToArray();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}, Page: {3}, Count: {4}\r\nException: {5}",
					"RepositoryService",
					"public IEnumerable<UserRouteResult> LoadUserRouteByUserID(Guid userID, int page = 0, int count = 10)",
					userID,
					page,
					count,
					ex.ToString()
				);
			}

			return new UserRouteResult[] { };
		}

		public IEnumerable<UserRouteResult> LoadUserRouteByUserID(string openID, int page = 0, int count = 10, bool inForce = false)
		{
			try
			{
				var lst = new List<UserRouteResult>();
				var urs = UserRouteRepository.Instance.LoadByUserID(openID, page, count, inForce);

				if (urs != null && urs.Count() > 0)
				{
					foreach (var ur in urs)
					{
						lst.Add(new UserRouteResult(ur));
					}
				}

				return lst.ToArray();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: OpenID: {2}, Page: {3}, Count: {4}\r\nException: {5}",
					"RepositoryService",
					"public IEnumerable<UserRouteResult> LoadUserRouteByUserID(string openID, int page = 0, int count = 10)",
					openID,
					page,
					count,
					ex.ToString()
				);
			}

			return new UserRouteResult[] { };
		}
		#endregion

		#region "Methods: GetUserRouteLatest"
		public UserRouteResult GetUserRouteNewest(long userID, bool inForce = false)
		{
			try
			{
				var urs = UserRouteRepository.Instance.LoadByUserID(userID, 0, 10, inForce);

				return (urs != null && urs.Count() > 0) ? (new UserRouteResult(urs.First())) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"UserRouteResult GetUserRouteNewest(long userID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", userID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public UserRouteResult GetUserRouteNewest(Guid userID, bool inForce = false)
		{
			try
			{
				var urs = UserRouteRepository.Instance.LoadByUserID(userID, 0, 10, inForce);

				return (urs != null && urs.Count() > 0) ? (new UserRouteResult(urs.First())) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"public UserRouteResult GetUserRouteNewest(Guid userID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", userID, inForce),
					ex.ToString()
				);
			}

			return null;
		}

		public UserRouteResult GetUserRouteNewest(string openID, bool inForce)
		{
			try
			{
				var urs = UserRouteRepository.Instance.LoadByUserID(openID, 0, 10, inForce);

				return (urs != null && urs.Count() > 0) ? (new UserRouteResult(urs.First())) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"public UserRouteResult GetUserRouteNewest(string openID, bool inForce = false)",
					string.Format("UserID: {0}, InForce: {1}", openID, inForce),
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: LoadUserRouteNewest"
		public IEnumerable<UserRouteResult> LoadUserRouteNewest(RouteSearchRequest requestInfo, bool inForce = false)
		{
			try
			{
				var t1 = DateTime.Now.Ticks;
				var results = UserRouteRepository.Instance.LoadNewest(requestInfo.Filter.LocationFilter.From_City, requestInfo.Page ?? 0, requestInfo.Count ?? 10, inForce);

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat(
						"Service: [{0}], Method: {1}\r\nParameters: [{2}]\r\nResults: Return [{3}] record(s) in [{4}] second(s)",
						"RepositoryService",
						"IEnumerable<UserRouteResult> LoadUserRouteNewest(RouteSearchRequest requestInfo, bool inForce)",
						string.Format("RequestInfo: {0}, InForce: {1}", requestInfo.ToString(), inForce),
						results.Count(),
						TimeSpan.FromTicks(DateTime.Now.Ticks - t1).TotalSeconds
					);
				}

				return results;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"IEnumerable<UserRouteResult> LoadUserRouteNewest(RouteSearchRequest requestInfo, bool inForce)",
					string.Format("{0}, InForce: {1}", requestInfo.ToString(), inForce),
					ex.ToString()
				);
			}

			return new UserRouteResult[] { };
		} 

		public IEnumerable<UserRouteResult> LoadUserRouteNewest4Disp(string city, bool inForce = false)
		{
			try
			{
				return UserRouteRepository.Instance.LoadNewest4Disp(city, inForce);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"IEnumerable<UserRouteResult> LoadUserRouteNewest4Disp(string city, bool inForce = false)",
					string.Format("City: {0}, InForce: {1}", city, inForce),
					ex.ToString()
				);
			}

			return new UserRouteResult[] { };
		}
		#endregion
	}
}
