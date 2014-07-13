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
	public class PhoneVerify
	{
		#region "Fields"
		List<PhoneVerifyItem> _lst = new List<PhoneVerifyItem>();
		#endregion

		#region "Properties"
		[DataMember]
		public int Mobile { get; set; }

		[DataMember]
		public List<PhoneVerifyItem> Matrixs { get { return _lst; } }
		#endregion
	}

	public class PhoneVerifyItem
	{
		#region "Constructs"
		public PhoneVerifyItem() { }

		public PhoneVerifyItem(DataStores.tbl_Common_PhoneVerify entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);

			this.Status = (entity.Status.HasValue) ? ((PhoneVerifyStatus)Enum.Parse(typeof(PhoneVerifyStatus), entity.Status.Value.ToString())) : PhoneVerifyStatus.UnVerified;
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public string Phonenum { get; set; }

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public int Channel { get; set; }

		[DataMember]
		public Nullable<int> Count { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public PhoneVerifyStatus Status { get; set; } 
		#endregion
	}
}
