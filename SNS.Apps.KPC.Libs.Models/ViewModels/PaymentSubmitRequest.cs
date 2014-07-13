using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class PaymentSubmitRequest
	{
		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public Nullable<int> PayMethod { get; set; }

		[DataMember]
		public PaymentStatus PayStatus { get; set; }

		[DataMember]
		public Nullable<decimal> PayAmount { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("OrderID: {0}, PayMethod: {1}, PayAmount: {2}, PayStatus: {3}", this.ID, this.PayMethod, (this.PayAmount.HasValue ? this.PayAmount.Value.ToString("D2") : "0.00"), this.PayStatus);
		} 
		#endregion
	}
}
