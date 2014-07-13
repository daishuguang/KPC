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
	public abstract class BaseRecord
	{
		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Memo { get; set; }
		#endregion
	}
}
