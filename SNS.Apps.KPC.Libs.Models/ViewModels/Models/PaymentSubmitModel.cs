using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class PaymentSubmitModel
	{
		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string Folio { get; set; }

		[DataMember]
		public PaymentMethod PayMethod { get; set; }

		public Nullable<decimal> PayAmount { get; set; }
		#endregion
	}
}
