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
	public class RouteEditModel
	{
		#region "Constructs"
		public RouteEditModel() { }

		public RouteEditModel(Route entity)
		{
			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public Guid RouteGUID { get; set; }

		#region "From Location"
		[DataMember]
		public string From_Province { get; set; }

		[DataMember]
		public string From_City { get; set; }

		[DataMember]
		public string From_District { get; set; }

		[Display(Name = "起始地")]
		[StringLength(300)]
		[DataMember]
		public string From_Location { get; set; }

		[DataMember]
		public Nullable<double> From_Longitude { get; set; }

		[DataMember]
		public Nullable<double> From_Latitude { get; set; }
		#endregion

		#region "To Location"
		[DataMember]
		public string To_Province { get; set; }

		[DataMember]
		public string To_City { get; set; }

		[DataMember]
		public string To_District { get; set; }

		[Display(Name = "目的地")]
		[StringLength(300)]
		[DataMember]
		public string To_Location { get; set; }

		[DataMember]
		public Nullable<double> To_Longitude { get; set; }

		[DataMember]
		public Nullable<double> To_Latitude { get; set; }
		#endregion

		[DataMember]
		public Nullable<System.DateTime> StartDate { get; set; }

		[DataMember]
		public Nullable<decimal> Charge { get; set; }

		[DataMember]
		public Nullable<int> SeatCount { get; set; }

		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public Nullable<bool> IsLongTerm { get; set; }

		[DataMember]
		public RouteType RouteType { get; set; }

		[DataMember]
		public RouteStatus Status { get; set; } 
		#endregion
	}
}
