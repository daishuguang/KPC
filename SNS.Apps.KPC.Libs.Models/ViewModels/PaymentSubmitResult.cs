using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class PaymentSubmitResult
	{
		#region "Constructs"
		public PaymentSubmitResult() { }

		public PaymentSubmitResult(DataStores.tbl_User_Payment entity)
		{
			if (entity != null)
			{
				this.ID = entity.OrderID;
				this.PayAmount = entity.PayAmount;
				this.PayFolio = entity.PayFolio;
				this.PayMethod = entity.PayMethod;
				this.PayStatus = (entity.PayStatus.HasValue) ? ((PaymentStatus)Enum.Parse(typeof(PaymentStatus), entity.PayStatus.Value.ToString())) : (PaymentStatus.Pending);
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string PayFolio { get; set; }

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
