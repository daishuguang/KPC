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
	public class UserOrderCreateRequest
	{
		#region "Properties"
		[DataMember]
		public Guid RequestorID { get; set; }

		[DataMember]
		public UserRole RequestorRole { get; set; }

		[DataMember]
		public Guid SupplierID { get; set; }

		[DataMember]
		public UserRole SupplierRole { get; set; }

		[DataMember]
		public Guid RouteID { get; set; }

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
			return string.Format("RequestorID: {0}, RequestorRole: {1}, SupplierID: {2}, RouteID: {3}", this.RequestorID, this.RequestorRole.ToString(), this.SupplierID, this.RouteID);
		} 
		#endregion
	}
}
