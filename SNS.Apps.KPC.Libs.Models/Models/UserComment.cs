using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserComment
	{
		#region "Constructs"
		public UserComment() { }

		public UserComment(DataStores.tbl_User_Comment entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);

			this.Content = entity.Comments;
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public long UserID { get; set; }

		[DataMember]
		public long RouteID { get; set; }

		[DataMember]
		public long ScoreBy { get; set; }

		[DataMember]
		public Nullable<int> ScoreRate { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public Nullable<System.DateTime> CreateDate { get; set; }
		#endregion
	}
}
