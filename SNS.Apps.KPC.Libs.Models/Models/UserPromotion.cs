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
	public class UserPromotion
	{
		[DataMember]
		public Nullable<long> UserID { get; set; }

		#region "Properties: 代理"
		[DataMember]
		public int SceneID { get; set; }

		[DataMember]
		public string SceneName { get; set; }
		#endregion

		#region "Properties: 推广"
		[DataMember]
		public int PromCode { get; set; }
		#endregion

		#region "Properties: 平台合作"
		[DataMember]
		public int ExtendChannel { get; set; }

		[DataMember]
		public string ExtendName { get; set; }
		#endregion
	}
}
