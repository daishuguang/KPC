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
	public class UserTrack
	{
		#region "Constructs"
		public UserTrack() { }

		public UserTrack(DataStores.tbl_User_Track entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);
		}

		public UserTrack(DataStores.sp_Load_Available_UserTracks_Result entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);
		}
		#endregion

		#region "Properties"
		//[DataMember]
		//public long ID { get; set; }

		[DataMember]
		public Nullable<long> UserID { get; set; }

		[DataMember]
		public Nullable<double> Longitude { get; set; }

		[DataMember]
		public Nullable<double> Latitude { get; set; }
		#endregion

		#region "Properties"
		[IgnoreDataMember]
		public Nullable<Point> Position { get { return new Point { Longitude = this.Longitude, Latitude = this.Latitude }; } }

		[DataMember]
		public Nullable<double> Distance { get; set; }
		#endregion
	}
}
