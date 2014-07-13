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
	public class UserPromotionExecuteResult
	{
		#region "Constructs"
		public UserPromotionExecuteResult() { }

		public UserPromotionExecuteResult(DataStores.sp_Set_PromotionUser_Result entity) 
		{
 			if (entity != null)
			{
				this.Status = entity.Status;

				if (this.Status == 0)
				{
					this.UserID = entity.ID;
					this.UserGUID = Guid.Parse(entity.UserGUID);
					this.OpenID = entity.OpenID;
					this.NickName = entity.NickName;
				}
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public int Status { get; set; }

		[DataMember]
		public Nullable<long> UserID { get; set; }

		[DataMember]
		public Nullable<Guid> UserGUID { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string NickName { get; set; } 
		#endregion
	}
}
