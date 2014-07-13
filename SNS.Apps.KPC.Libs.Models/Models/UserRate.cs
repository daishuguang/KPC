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
	public class UserRate
	{
		#region "Contructs"
		public UserRate() { }

		public UserRate(DataStores.tbl_User_Rate entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public long UserID { get; set; }

		[DataMember]
		public Nullable<int> GoodCount { get; set; }

		[DataMember]
		public Nullable<int> BadCount { get; set; }

		[DataMember]
		public Nullable<System.DateTime> CreateDate { get; set; }

		[DataMember]
		public Nullable<System.DateTime> UpdateDate { get; set; } 
		#endregion
	}
}
