using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserPromotionStatisticsResult
	{
		#region "Constructs"
		public UserPromotionStatisticsResult() { }

		public UserPromotionStatisticsResult(DataStores.sp_Get_PromotionCount_Result entity)
		{
			if (entity != null)
			{
				this.Status = entity.Status;

				if (entity.Status == 0)
				{
					this.Total_Count = entity.Total;
					this.Register_Count = entity.Registerted;
					this.Subscribe_Count = entity.Subscribed;
					this.Unsubscribe_Count = entity.Unsubscribed;
				}
			}
		}

		public UserPromotionStatisticsResult(IEnumerable<DataStores.sp_Load_PromotionUsers_Result> entity)
		{
			if (entity != null)
			{
				this.Status = 0;

				//
				var lst = new List<UserPromotionConsumer>();

				foreach(var item in entity)
				{
					lst.Add(new UserPromotionConsumer(item));
				}

				this.Consumers = lst.ToArray();
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public int Register_Count { get; set; }

		[DataMember]
		public int Subscribe_Count { get; set; }

		[DataMember]
		public int Unsubscribe_Count { get; set; }

		[DataMember]
		public int Total_Count { get; set; }

		[DataMember]
		public int Status { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public IEnumerable<UserPromotionConsumer> Consumers { get; set; } 
		#endregion
	}

	[DataContract]
	public class UserPromotionConsumer
	{
		#region "Constructs"
		public UserPromotionConsumer() { }

		public UserPromotionConsumer(DataStores.sp_Load_PromotionUsers_Result entity)
		{
			if (entity != null)
			{
				this.UserGUID = (!string.IsNullOrEmpty(entity.UserGUID)) ? (Guid.Parse(entity.UserGUID)) : (Guid.Empty);
				this.OpenID = entity.OpenID;
				this.NickName = entity.NickName;
				this.Mobile = entity.Mobile;
				this.Status = entity.UserStatus.HasValue ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.UserStatus.Value.ToString())) : UserStatus.Subscribed;
				this.CreateDate = entity.CreateDate;
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public Guid UserGUID { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public UserStatus Status { get; set; }

		[DataMember]
		public Nullable<DateTime> CreateDate { get; set; }
		#endregion
	}
}
