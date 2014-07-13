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
	public class UserOrderResult
	{
		#region "Constructs"
		public UserOrderResult() { }

		public UserOrderResult(UserOrder entity)
		{
			if (entity != null)
			{
				this.ID = entity.ID;
				this.Folio = entity.Folio;

				this.Requestor = entity.Requestor;
				this.RequestorRole = entity.RequestorRole;

				this.Supplier = entity.Supplier;
				this.SupplierRole = entity.SupplierRole;
				this.Route = entity.Route;

				this.StartDate = entity.StartDate;
				this.Charge = entity.Charge;
				this.Note = entity.Note;
				this.Status = entity.Status;

				this.IsCancelled_Requestor = entity.IsCancelled_Requestor;
				this.IsCancelled_Supplier = entity.IsCancelled_Supplier;
				this.IsConfirmed_Requestor = entity.IsConfirmed_Requestor;
				this.IsConfirmed_Supplier = entity.IsConfirmed_Supplier;
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
		public Nullable<System.DateTime> StartDate { get; set; }

		[DataMember]
		public Nullable<decimal> Charge { get; set; }

		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public OrderStatus Status { get; set; }
		#endregion
	}
}
