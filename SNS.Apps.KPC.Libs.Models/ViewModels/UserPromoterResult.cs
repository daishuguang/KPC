using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserPromoterResult
	{
		#region "Constructs"
		public UserPromoterResult() { }

		public UserPromoterResult(DataStores.tbl_User entity, UserPromotionType channelType, string channelCode, string channelMemo)
		{
			if (entity != null)
			{
				this.Promoter = new UserPromoter(entity);
				this.Channels = new UserPromotionChannel[] { new UserPromotionChannel { ChannelType = channelType, ChannelCode = channelCode } };
			}
		}

		public UserPromoterResult(DataStores.tbl_User entity, IEnumerable<DataStores.tbl_User_Promotion> channels)
		{
			if (entity != null && channels != null)
			{
				this.Promoter = new UserPromoter(entity);

				var lstChannels = new List<UserPromotionChannel>();

				foreach (var channel in channels)
				{
					lstChannels.Add(new UserPromotionChannel(channel));
				}

				this.Channels = lstChannels.ToArray();
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public UserPromoter Promoter { get; set; }

		[DataMember]
		public IEnumerable<UserPromotionChannel> Channels { get; set; }
		#endregion

		#region "Methods"
		public bool CheckIsRef(UserPromotionType channelType, string channelCode)
		{
			if (this.Channels == null || this.Channels.Count() == 0)
			{
				return false;
			}

			foreach (var channel in this.Channels)
			{
				if (channel.ChannelType == channelType && string.Compare(channel.ChannelCode, channelCode) == 0)
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}

	[DataContract]
	public class UserPromoter
	{
		#region "Constructs"
		public UserPromoter(DataStores.tbl_User entity)
		{
			if (entity != null)
			{
				Utils.CommonUtility.CopyTo(entity, this);

				this.Status = entity.Status != null && entity.Status.HasValue ? (UserStatus)Enum.Parse(typeof(UserStatus), entity.Status.Value.ToString()) : UserStatus.Subscribed;
			}
		}

		public UserPromoter(User entity)
		{
			if (entity != null)
			{
				Utils.CommonUtility.CopyTo(entity, this);
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public Guid UserGUID { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }

		[DataMember]
		public string PortraitsThumbUrl { get; set; }

		[DataMember]
		public UserStatus Status { get; set; }

		[DataMember]
		public Nullable<long> RefID { get; set; }
		#endregion
	}

	[DataContract]
	public class UserPromotionChannel
	{
		#region "Constructs"
		public UserPromotionChannel() { }

		public UserPromotionChannel(DataStores.tbl_User_Promotion entity)
		{
			this.ChannelType = (UserPromotionType)Enum.Parse(typeof(UserPromotionType), entity.ChannelID.Value.ToString());
			this.ChannelCode = entity.ChannelCode;
			this.ChannelMemo = entity.ChannelMemo;
		}
		#endregion

		#region "Properties"
		[DataMember]
		public UserPromotionType ChannelType { get; set; }

		[DataMember]
		public string ChannelCode { get; set; }

		[DataMember]
		public string ChannelMemo { get; set; }
		#endregion
	}
}
