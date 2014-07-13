using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserOrderRequest : ViewModels.PagedRequest
	{
		#region "Properties"
		[DataMember]
		public UserOrderRequestFilter Filter { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Filter: [{0}], Page: {1}, Count: {2}", this.Filter.ToString(), (this.Page.HasValue ? this.Page.Value.ToString() : "0"), (this.Count.HasValue ? this.Count.Value.ToString() : "10"));
		} 
		#endregion

		#region "Internal Class"
		[DataContract]
		public class UserOrderRequestFilter
		{
			#region "Properties"
			[DataMember]
			public Nullable<Guid> UserID { get; set; }

			[DataMember]
			public Nullable<Guid> RouteID { get; set; }

			[DataMember]
			public Nullable<OrderType> OrderType { get; set; }

			[DataMember]
			public Nullable<OrderStatus> OrderStatus { get; set; }
			#endregion

			#region "Methods"
			public override string ToString()
			{
				return string.Format("UserID: {0}, RouteID: {1}, OrderType: {2}, OrderStatus: {3}",
					(this.UserID.HasValue ? this.UserID.Value.ToString() : string.Empty),
					(this.RouteID.HasValue ? this.RouteID.Value.ToString() : string.Empty),
					(this.OrderType.HasValue ? this.OrderType.Value.ToString() : SNS.Apps.KPC.Libs.Models.OrderType.All.ToString()),
					(this.OrderStatus.HasValue ? this.OrderStatus.Value.ToString() : SNS.Apps.KPC.Libs.Models.OrderStatus.PendingForPayment.ToString())
				);
			}
			#endregion
		} 
		#endregion
	}
}
