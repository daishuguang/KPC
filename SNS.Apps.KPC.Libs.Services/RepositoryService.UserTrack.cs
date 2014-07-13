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
		#region "UserTrack Services"

		#region "Methods: GetUserTrack"
		public UserTrackResult GetUserTrack(long userID, bool inForce = false)
		{
			try
			{
				var ut = UserTrackRepository.Instance.GetUserTrack(userID);

				return (ut != null) ? (new UserTrackResult
				{
					User = UserRepository.Instance.Get(userID),
					Route = (UserRouteRepository.Instance.GetByUserID(userID) != null) ? (UserRouteRepository.Instance.GetByUserID(userID).Route) : (null),
					Position = ut.Position
				}) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: UserID: {2}\r\nException: {3}",
					"RouteMatrixService",
					"UserTrackResult GetUserTrack(long userID)",
					userID,
					ex.ToString()
				);
			}

			return null;
		}

		public UserTrackResult GetUserTrack(Guid userID, bool inForce = false)
		{
			try
			{
				var ut = UserTrackRepository.Instance.GetUserTrack(userID);
				
				return (ut != null) ? (new UserTrackResult 
				{ 
					User = UserRepository.Instance.Get(userID),
					Route = (UserRouteRepository.Instance.GetByUserID(userID) != null) ? (UserRouteRepository.Instance.GetByUserID(userID).Route) : (null),
					Position = ut.Position
				}) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: UserID: {2}\r\nException: {3}",
					"RouteMatrixService",
					"UserTrackResult GetUserTrack(Guid userID)",
					userID,
					ex.ToString()
				);
			}

			return null;
		}

		public UserTrackResult GetUserTrack(string openID, bool inForce = false)
		{
			try
			{
				var ut = UserTrackRepository.Instance.GetUserTrack(openID);

				return (ut != null) ? (new UserTrackResult
				{
					User = UserRepository.Instance.Get(openID),
					Route = (UserRouteRepository.Instance.GetByUserID(openID) != null) ? (UserRouteRepository.Instance.GetByUserID(openID).Route) : (null),
					Position = ut.Position
				}) : (null);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: OpenID: {2}\r\nException: {3}",
					"RouteMatrixService",
					"UserTrackResult GetUserTrack(string openID)",
					openID,
					ex.ToString()
				);
			}

			return null;
		}
		#endregion

		#region "Methods: SetUserTrack"
		public void SetUserTrack(long userID, Point position)
		{
			try
			{
				UserTrackRepository.Instance.SetUserTrack(userID, position);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"void SetUserTrack(long userID, Point position)",
					string.Format("UserID: {0}, Position: {1}", userID, position),
					ex.ToString()
				);
			}
		}

		public void SetUserTrack(Guid userID, Point position)
		{
			try
			{
				UserTrackRepository.Instance.SetUserTrack(userID, position);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"void SetUserTrack(Guid userID, Point position)",
					string.Format("UserID: {0}, Position: {1}", userID, position),
					ex.ToString()
				);
			}
		}

		public void SetUserTrack(string openID, Point position)
		{
			try
			{
				UserTrackRepository.Instance.SetUserTrack(openID, position);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: {1}\r\nParameters: UserID: {2}\r\nException: {3}",
					"RepositoryService",
					"void SetUserTrack(string openID, Point position)",
					string.Format("OpenID: {0}, Position: {1}", openID, position),
					ex.ToString()
				);
			}
		}
		#endregion

		#region "Methods: LoadUserAround"
		public IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestList requestInfo)
		{
			try
			{
				return UserTrackRepository.Instance.LoadUserAroundList(requestInfo.Filter, requestInfo.Page ?? 0, requestInfo.Count ?? 10);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: UserTrackRequestList: {2}\r\nException: {3}",
					"RouteSearchService",
					"IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestList requestInfo)",
					string.Format("[RequestInfo: {0}]", requestInfo.ToString()),
					ex.ToString()
				);
			}

			return new UserTrackResult[] { };
		}

		public IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestMap requestInfo)
		{
			try
			{
				return UserTrackRepository.Instance.LoadUserAroundMap(requestInfo.Filter);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: UserTrackRequestMap: {2}\r\nException: {3}",
					"RouteSearchService",
					"IEnumerable<UserTrackResult> LoadUserAround(UserTrackRequestMap requestInfo)",
					string.Format("[RequestInfo: {0}]", requestInfo.ToString()),
					ex.ToString()
				);
			}

			return new UserTrackResult[] { };
		}
		#endregion

		#endregion
	}
}
