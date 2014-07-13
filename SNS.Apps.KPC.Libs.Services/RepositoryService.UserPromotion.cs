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
		#region "代理/推荐/平台合作"

		#region "Methods: GetPromoter"
		public UserPromoterResult GetPromoter(long userID)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromoter(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterResult GetPromoter(long userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(Guid userID)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromoter(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterResult GetPromoter(Guid userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(string openID)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromoter(openID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterResult GetPromoter(string openID)",
					string.Format("OpenID: {0}", openID),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(long? refID)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromoter(refID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterResult GetPromoter(long? refID)",
					string.Format("RefID: {0}", refID),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(UserPromotionType channelType, string channelCode)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromoter(channelType, channelCode);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterResult GetPromoter(UserPromotionType channelType, string channelCode)",
					string.Format("ChannelType: {0}, ChannelCode: {1}", channelType, channelCode),
					ex.ToString()
				);
			}

			return null;
		} 
		#endregion

		public string GetPromotionCode(long userID, UserPromotionType channelType)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromotionCode(userID, channelType);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"string GetPromotionCode(long userID, UserPromotionType channelType)",
					string.Format("UserID: {0}, ChannelType", userID, channelType),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromotionStatisticsResult GetPromotionCount(long userID)
		{
			try
			{
				return UserPromotionRepository.Instance.GetPromotionCount(userID);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromotionStatisticsResult GetPromotionCount(long userID)",
					string.Format("UserID: {0}", userID),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromotionStatisticsResult LoadPromotionUsers(long userID, int maxCount = 10)
		{
			try
			{
				return UserPromotionRepository.Instance.LoadPromotionUsers(userID, maxCount);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromotionStatisticsResult LoadPromotionUsers(long userID, int maxCount = 10)",
					string.Format("UserID: {0}, MaxCount: {1}", userID, maxCount),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromotionExecuteResult SetPromotionUser(long userID, int code)
		{
			try
			{
				return UserPromotionRepository.Instance.SetPromotionUser(userID, code);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromotionExecuteResult SetPromotionUser(long userID, int code)",
					string.Format("UserID: {0}, Code: {1}", userID, code),
					ex.ToString()
				);
			}

			return null;
		}

		public UserPromoterRankResult LoadPromoterRank()
		{
			try
			{
				return UserPromotionRepository.Instance.LoadPromoterRank();
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"RepositoryService",
					"UserPromoterRankResult LoadPromoterRank",
					string.Empty,
					ex.ToString()
				);
			}

			return null;
		}
		#endregion
	}
}
