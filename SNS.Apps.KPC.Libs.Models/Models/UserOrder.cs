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
	public class UserOrder
	{
		#region "Constructs"
		public UserOrder() { }

		public UserOrder(DataStores.tbl_User_Order entity) 
		{
 			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);

				this.RequestorRole = (UserRole)Enum.Parse(typeof(UserRole), entity.RequestorRole.ToString());
				this.SupplierRole = (UserRole)Enum.Parse(typeof(UserRole), entity.SupplierRole.ToString());

				this.Status = (entity.Status.HasValue) ? ((OrderStatus)Enum.Parse(typeof(OrderStatus), entity.Status.Value.ToString())) : (OrderStatus.PendingForPayment);
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string Folio { get; set; }

		[DataMember]
		public User Requestor { get; set; }

		[DataMember]
		public UserRole RequestorRole { get; set; }

		[DataMember]
		public User Supplier { get; set; }

		[DataMember]
		public UserRole SupplierRole { get; set; }

		[DataMember]
		public Route Route { get; set; }

		[DataMember]
		public Nullable<bool> IsConfirmed_Requestor { get; set; }

		[DataMember]
		public Nullable<bool> IsConfirmed_Supplier { get; set; }

		[DataMember]
		public Nullable<bool> IsCancelled_Requestor { get; set; }

		[DataMember]
		public Nullable<bool> IsCancelled_Supplier { get; set; }

		[DataMember]
		public Nullable<DateTime> StartDate { get; set; }

		[DataMember]
		public Nullable<decimal> Charge { get; set; }

		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public OrderStatus Status { get; set; }

		[DataMember]
		public Nullable<DateTime> CreateDate { get; set; }

		[DataMember]
		public Nullable<DateTime> UpdateDate { get; set; }
		#endregion
	}
}
