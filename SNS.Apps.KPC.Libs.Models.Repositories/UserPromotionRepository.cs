using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public class UserPromotionRepository : Base.RepositoryBase<UserPromotionRepository>
	{
		#region "Constructs"
		private UserPromotionRepository() { } 
		#endregion

		#region "Methods: GetPromoter"
		public UserPromoterResult GetPromoter(long userID)
		{
			var user = UserRepository.Instance.Get(userID);

			if (user.RefID.HasValue)
			{
				return GetPromoter(user.RefID.Value);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(Guid userID)
		{
			var user = UserRepository.Instance.Get(userID);

			if (user.RefID.HasValue)
			{
				return GetPromoter(user.RefID.Value);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(string openID)
		{
			var user = UserRepository.Instance.Get(openID);

			if (user.RefID.HasValue)
			{
				return GetPromoter(user.RefID.Value);
			}

			return null;
		}

		public UserPromoterResult GetPromoter(long? refID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Promotion.SingleOrDefault(p => p.ID == refID && p.UserID.HasValue && !string.IsNullOrEmpty(p.ChannelCode));

				if (entity != null)
				{
					var entity_user = ctx.tbl_User.SingleOrDefault(p => p.ID == entity.UserID.Value);
					var entity_channel = ctx.tbl_User_Promotion.Where(p => p.UserID == entity.UserID.Value && p.ChannelID != null && p.ChannelID.HasValue && !string.IsNullOrEmpty(p.ChannelCode));

					if (entity_user != null && entity_channel != null)
					{
						return new UserPromoterResult(entity_user, entity_channel);
					}
				}
			}

			return null;
		}

		public UserPromoterResult GetPromoter(UserPromotionType channelType, string channelCode)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Promotion.SingleOrDefault(p => p.ChannelID.Value == (long)channelType && string.Compare(p.ChannelCode, channelCode) == 0 && p.UserID.HasValue);

				if (entity != null)
				{
					var entity_user = ctx.tbl_User.SingleOrDefault(p => p.ID == entity.UserID.Value);
					var entity_channel = ctx.tbl_User_Promotion.Where(p => p.UserID == entity.UserID.Value && p.ChannelID != null && p.ChannelID.HasValue && !string.IsNullOrEmpty(p.ChannelCode));

					if (entity_user != null && entity_channel != null)
					{
						return new UserPromoterResult(entity_user, entity_channel);
					}
				}
			}

			return null;
		}
		#endregion

		#region "Methods: GetPromotionCode"
		public string GetPromotionCode(long userID, UserPromotionType channelType)
		{
			using (var ctx = new Libs.Models.DataStores.KPCDataModels())
			{
				var result = ctx.sp_Get_PromotionCode(userID, (int)channelType).FirstOrDefault();

				return (result != null && result.Status == 0) ? (result.Code) : (null);
			}
		} 
		#endregion

		#region "Methods: GetPromotionCount"
		public UserPromotionStatisticsResult GetPromotionCount(long userID)
		{
			using (var ctx = new Libs.Models.DataStores.KPCDataModels())
			{
				var result = ctx.sp_Get_PromotionCount(userID).FirstOrDefault();

				if (result != null)
				{
					return new UserPromotionStatisticsResult(result);
				}
			}

			return null;
		} 
		#endregion

		#region "Methods: LoadPromotionUsers"
		public UserPromotionStatisticsResult LoadPromotionUsers(long userID, int maxCount)
		{
			using (var ctx = new Libs.Models.DataStores.KPCDataModels())
			{
				var result = ctx.sp_Load_PromotionUsers(userID, maxCount);

				if (result != null)
				{
					return (new UserPromotionStatisticsResult(result));
				}
			}

			return null;
		}
		#endregion

		#region "Methods: SetPromotionUser"
		public UserPromotionExecuteResult SetPromotionUser(long userID, int code)
		{
			using (var ctx = new Libs.Models.DataStores.KPCDataModels())
			{
				var result = ctx.sp_Set_PromotionUser(userID, code).FirstOrDefault();

				if (result != null)
				{
					return new UserPromotionExecuteResult(result);
				}

				return null;
			}
		}
		#endregion

		#region "Methods: LoadPromoterRank"
		public UserPromoterRankResult LoadPromoterRank()
		{
			using (var ctx = new Libs.Models.DataStores.KPCDataModels())
			{
				var result = ctx.sp_Load_Promoters();

				if (result != null)
				{
					return new UserPromoterRankResult(result);
				}

				return null;
			}
		} 
		#endregion
	}
}
