using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserOrderUpdateRequest
	{
		#region "Constructs"
		public UserOrderUpdateRequest() { }

		public UserOrderUpdateRequest(string folio) { this.Folio = folio; } 
		#endregion

		#region "Properties"
		[DataMember]
		public string Folio { get; set; }

		[DataMember]
		public Nullable<DateTime> StartDate { get; set; }

		[DataMember]
		public Nullable<decimal> Charge { get; set; }

		[DataMember]
		public string Note { get; set; }   
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("RequestorID: {0}, RequestorRole: {1}, SupplierID: {2}, RouteID: {3}", this.Folio, (this.StartDate != null && this.StartDate.HasValue ? this.StartDate.Value.ToString("yyyy/MM/dd HH:mm") : "NULL"), (this.Charge != null && this.Charge.HasValue ? this.Charge.Value.ToString("D4") : "面议"));
		}
		#endregion
	}
}
