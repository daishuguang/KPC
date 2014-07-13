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
	public class UserPromoterRankResult
	{
		#region "Constructs"
		public UserPromoterRankResult(){}

		public UserPromoterRankResult(IEnumerable<DataStores.sp_Load_Promoters_Result> entity) 
		{
			var lst = new List<UserPromoterRank>();

			if (entity != null)
			{
				foreach(var item in entity)
				{
					lst.Add(new UserPromoterRank(item));
				}
			}

			this.Promoters = lst.ToArray();
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public IEnumerable<UserPromoterRank> Promoters { get; set; } 
		#endregion
	}

	[DataContract]
	public class UserPromoterRank
	{
		#region "Constructs"
		public UserPromoterRank() { }

		public UserPromoterRank(DataStores.sp_Load_Promoters_Result entity)
		{
			if (entity != null)
			{
				this.UserID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.NickName;
				this.PortraitsUrl = entity.PortraitsUrl;
				this.PortraitsThumbUrl = entity.PortraitsThumbUrl;
				this.Registerted = entity.Registerted.HasValue ? entity.Registerted.Value : 0;
				this.Subscribed = entity.Subscribed.HasValue ? entity.Subscribed.Value : 0;
				this.Unsubcribed = entity.Unsubscribed.HasValue ? entity.Unsubscribed.Value : 0;
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public Guid UserID { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }

		[DataMember]
		public string PortraitsThumbUrl { get; set; }

		[DataMember]
		public int Registerted { get; set; }

		[DataMember]
		public int Subscribed { get; set; }

		[DataMember]
		public int Unsubcribed { get; set; } 
		#endregion
	}
}
