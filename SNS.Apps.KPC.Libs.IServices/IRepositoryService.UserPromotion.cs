using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Libs.IServices
{
	public partial interface IRepositoryService
	{
		#region "Methods: GetPromoter"
		[OperationContract(Name = "GetPromoterByUserID")]
		UserPromoterResult GetPromoter(long userID);

		[OperationContract(Name = "GetPromoterByUserGUID")]
		UserPromoterResult GetPromoter(Guid userID);

		[OperationContract(Name = "GetPromoterByOpenID")]
		UserPromoterResult GetPromoter(string openID);

		[OperationContract(Name = "GetPromoterByRefID")]
		UserPromoterResult GetPromoter(long? refID);

		[OperationContract(Name = "GetPromoterByChannelCode")]
		UserPromoterResult GetPromoter(UserPromotionType channelType, string channelCode); 
		#endregion

		[OperationContract]
		string GetPromotionCode(long userID, UserPromotionType channelType);

		[OperationContract]
		UserPromotionStatisticsResult GetPromotionCount(long userID);

		[OperationContract]
		UserPromotionStatisticsResult LoadPromotionUsers(long userID, int maxCount = 10);

		[OperationContract]
		UserPromotionExecuteResult SetPromotionUser(long userID, int code);

		[OperationContract]
		UserPromoterRankResult LoadPromoterRank();
	}
}
