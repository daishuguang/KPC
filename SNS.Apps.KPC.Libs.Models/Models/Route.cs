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
	public class Route
	{
		#region "Constructs"
		public Route() { }

		public Route(DataStores.tbl_Route entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);

			this.RouteGUID = Guid.Parse(entity.RouteGUID);
			//this.RepeatType = (entity.RepeatType.HasValue) ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : (RouteRepeatType.NoRepeat);
			//this.RepeatCondition = (entity.RouteType.HasValue) ? ((RouteRepeatCondition)Enum.Parse(typeof(RouteRepeatCondition), entity.RouteType.Value.ToString())) : (RouteRepeatCondition.Repeat_None);
			this.RouteType = (entity.RouteType.HasValue) ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : (RouteType.Citywide_Workday);
			this.Status = (entity.Status.HasValue) ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Status.Value.ToString())) : (RouteStatus.Available);
		}

		public Route(DataStores.sp_Load_Available_Routes_Result entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);

			this.RouteGUID = Guid.Parse(entity.RouteGUID);
			//this.RepeatType = (entity.RepeatType.HasValue) ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : (RouteRepeatType.NoRepeat);
			//this.RepeatCondition = (entity.RouteType.HasValue) ? ((RouteRepeatCondition)Enum.Parse(typeof(RouteRepeatCondition), entity.RouteType.Value.ToString())) : (RouteRepeatCondition.Repeat_None);
			this.RouteType = (entity.RouteType.HasValue) ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : (RouteType.Citywide_Workday);
			this.Status = (entity.Status.HasValue) ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Status.Value.ToString())) : (RouteStatus.Available);
		}

		public Route(Route entity)
		{
			Libs.Utils.CommonUtility.CopyTo(entity, this);
		}

		public Route(RouteSearchFilter filter)
		{
			#region "From"
			this.From_Location = filter.LocationFilter.From_Location;
			this.From_Province = filter.LocationFilter.From_Province;
			this.From_City = filter.LocationFilter.From_City;
			this.From_District = filter.LocationFilter.From_District;
			this.From_Longitude = filter.LocationFilter.From_Point.HasValue ? filter.LocationFilter.From_Point.Value.Longitude : 0;
			this.From_Latitude = filter.LocationFilter.From_Point.HasValue ? filter.LocationFilter.From_Point.Value.Latitude : 0;
			#endregion

			#region "To"
			this.To_Location = filter.LocationFilter.To_Location;
			this.To_Province = filter.LocationFilter.To_Province;
			this.To_City = filter.LocationFilter.To_City;
			this.To_District = filter.LocationFilter.To_District;
			this.To_Longitude = filter.LocationFilter.To_Point.HasValue ? filter.LocationFilter.To_Point.Value.Longitude : 0;
			this.To_Latitude = filter.LocationFilter.To_Point.HasValue ? filter.LocationFilter.To_Point.Value.Latitude : 0;
			#endregion

			// 同城
			//if (string.Compare(this.From_Province, this.To_Province, StringComparison.InvariantCultureIgnoreCase) == 0 && string.Compare(this.From_City, this.To_City, StringComparison.InvariantCultureIgnoreCase) == 0)
			//{
			//	this.RouteType = Models.RouteType.Citywide_All;
			//}
			//// 长途
			//else if (!string.IsNullOrEmpty(this.To_City))
			//{
			//	this.RouteType = Models.RouteType.Intercity_All;
			//}
			//// 全部
			//else
			//{
			//	this.RouteType = Models.RouteType.All;
			//}

			this.RouteType = filter.LocationFilter.RouteType;
		}

		public Route(DataStores.sp_Load_UserRoute_By_RouteID_Result entity)
		{
			this.ID = entity.RouteID;
			this.RouteGUID = Guid.Parse(entity.RouteGUID);

			#region "From_Location"
			this.From_Province = entity.From_Province;
			this.From_City = entity.From_City;
			this.From_District = entity.From_District;
			this.From_Location = entity.From_Location;
			this.From_Longitude = entity.From_Longitude;
			this.From_Latitude = entity.From_Latitude;
			#endregion

			#region "To_Location"
			this.To_Province = entity.To_Province;
			this.To_City = entity.To_City;
			this.To_District = entity.To_District;
			this.To_Location = entity.To_Location;
			this.To_Longitude = entity.To_Longitude;
			this.To_Latitude = entity.To_Latitude;
			#endregion

			this.StartDate = entity.StartDate;
			//this.RepeatType = (entity.RepeatType.HasValue) ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : (RouteRepeatType.NoRepeat);
			//this.RepeatCondition = entity.RepeatCondition;
			this.EndDate = entity.EndDate;
			this.Distance = entity.Distance;
			this.Charge = entity.Charge;
			this.SeatCount = entity.SeatCount;
			this.Note = entity.Note;
			this.CreateDate = entity.Route_CreateDate;
			this.UpdateDate = entity.Route_UpdateDate;

			this.IsExpired = entity.Route_IsExpired;
			//this.IsLongTerm = entity.Route_IsLongTerm;

			this.RouteType = (entity.RouteType.HasValue) ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : (RouteType.Citywide_Workday);
			this.Status = (entity.Route_Status.HasValue) ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Route_Status.Value.ToString())) : (RouteStatus.Available);
		}

		public Route(DataStores.sp_Load_UserRoute_By_RouteIDs_Result entity)
		{
			this.ID = entity.RouteID;
			this.RouteGUID = Guid.Parse(entity.RouteGUID);

			#region "From_Location"
			this.From_Province = entity.From_Province;
			this.From_City = entity.From_City;
			this.From_District = entity.From_District;
			this.From_Location = entity.From_Location;
			this.From_Longitude = entity.From_Longitude;
			this.From_Latitude = entity.From_Latitude;
			#endregion

			#region "To_Location"
			this.To_Province = entity.To_Province;
			this.To_City = entity.To_City;
			this.To_District = entity.To_District;
			this.To_Location = entity.To_Location;
			this.To_Longitude = entity.To_Longitude;
			this.To_Latitude = entity.To_Latitude;
			#endregion

			this.StartDate = entity.StartDate;
			//this.RepeatType = (entity.RepeatType.HasValue) ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : (RouteRepeatType.NoRepeat);
			//this.RepeatCondition = entity.RepeatCondition;
			this.EndDate = entity.EndDate;
			this.Distance = entity.Distance;
			this.Charge = entity.Charge;
			this.SeatCount = entity.SeatCount;
			this.Note = entity.Note;
			this.CreateDate = entity.Route_CreateDate;
			this.UpdateDate = entity.Route_UpdateDate;

			this.IsExpired = entity.Route_IsExpired;
			//this.IsLongTerm = entity.Route_IsLongTerm;

			this.RouteType = (entity.RouteType.HasValue) ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : (RouteType.Citywide_Workday);
			this.Status = (entity.Route_Status.HasValue) ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Route_Status.Value.ToString())) : (RouteStatus.Available);
		}

		public Route(DataStores.sp_Load_UserRoute_By_UserID_Result entity)
		{
			this.ID = entity.RouteID;
			this.RouteGUID = Guid.Parse(entity.RouteGUID);

			#region "From_Location"
			this.From_Province = entity.From_Province;
			this.From_City = entity.From_City;
			this.From_District = entity.From_District;
			this.From_Location = entity.From_Location;
			this.From_Longitude = entity.From_Longitude;
			this.From_Latitude = entity.From_Latitude;
			#endregion

			#region "To_Location"
			this.To_Province = entity.To_Province;
			this.To_City = entity.To_City;
			this.To_District = entity.To_District;
			this.To_Location = entity.To_Location;
			this.To_Longitude = entity.To_Longitude;
			this.To_Latitude = entity.To_Latitude;
			#endregion

			this.StartDate = entity.StartDate;
			//this.RepeatType = (entity.RepeatType.HasValue) ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : (RouteRepeatType.NoRepeat);
			//this.RepeatCondition = entity.RepeatCondition;
			this.EndDate = entity.EndDate;
			this.Distance = entity.Distance;
			this.Charge = entity.Charge;
			this.SeatCount = entity.SeatCount;
			this.Note = entity.Note;
			this.CreateDate = entity.Route_CreateDate;
			this.UpdateDate = entity.Route_UpdateDate;

			this.IsExpired = entity.Route_IsExpired;
			//this.IsLongTerm = entity.Route_IsLongTerm;

			this.RouteType = (entity.RouteType.HasValue) ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : (RouteType.Citywide_Workday);
			this.Status = (entity.Route_Status.HasValue) ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Route_Status.Value.ToString())) : (RouteStatus.Available);
		}

		public Route(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			this.ID = entity.RouteID;
			this.RouteGUID = Guid.Parse(entity.RouteGUID);

			#region "From_Location"
			this.From_Province = entity.From_Province;
			this.From_City = entity.From_City;
			this.From_District = entity.From_District;
			this.From_Location = entity.From_Location;
			this.From_Point = new Point { Longitude = entity.From_Longitude, Latitude = entity.From_Latitude };
			#endregion

			#region "To_Location"
			this.To_Province = entity.To_Province;
			this.To_City = entity.To_City;
			this.To_District = entity.To_District;
			this.To_Location = entity.To_Location;
			this.To_Point = new Point { Longitude = entity.To_Longitude, Latitude = entity.To_Latitude };
			#endregion

			this.StartDate = entity.StartDate;
			this.SeatCount = entity.SeatCount;
			this.Charge = entity.Charge;
			this.Note = entity.Note;
			this.Status = entity.Route_Status.HasValue ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Route_Status.Value.ToString())) : RouteStatus.Available;
			this.RouteType = entity.RouteType.HasValue ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : Models.RouteType.Citywide_Workday;

			this.IsExpired = entity.Route_IsExpired;
			//this.IsLongTerm = entity.Route_IsLongTerm;

			//this.RepeatCondition = (entity.RepeatCondition.HasValue) ? (entity.RepeatCondition.Value) : ((int)RouteRepeatCondition.Repeat_None);
			//this.RepeatType = entity.RepeatType.HasValue ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : RouteRepeatType.NoRepeat;
		}

		public Route(DataStores.sp_Load_UserRoute_Newest_Disp_Result entity)
		{
			this.ID = entity.RouteID;
			this.RouteGUID = Guid.Parse(entity.RouteGUID);

			#region "From_Location"
			this.From_Province = entity.From_Province;
			this.From_City = entity.From_City;
			this.From_District = entity.From_District;
			this.From_Location = entity.From_Location;
			this.From_Point = new Point { Longitude = entity.From_Longitude, Latitude = entity.From_Latitude };
			#endregion

			#region "To_Location"
			this.To_Province = entity.To_Province;
			this.To_City = entity.To_City;
			this.To_District = entity.To_District;
			this.To_Location = entity.To_Location;
			this.To_Point = new Point { Longitude = entity.To_Longitude, Latitude = entity.To_Latitude };
			#endregion

			this.StartDate = entity.StartDate;
			this.SeatCount = entity.SeatCount;
			this.Charge = entity.Charge;
			this.Note = entity.Note;
			this.Status = entity.Route_Status.HasValue ? ((RouteStatus)Enum.Parse(typeof(RouteStatus), entity.Route_Status.Value.ToString())) : RouteStatus.Available;
			this.RouteType = entity.RouteType.HasValue ? ((RouteType)Enum.Parse(typeof(RouteType), entity.RouteType.Value.ToString())) : Models.RouteType.Citywide_Workday;

			this.IsExpired = entity.Route_IsExpired;
			//this.IsLongTerm = entity.Route_IsLongTerm;

			//this.RepeatCondition = (entity.RepeatCondition.HasValue) ? (entity.RepeatCondition.Value) : ((int)RouteRepeatCondition.Repeat_None);
			//this.RepeatType = entity.RepeatType.HasValue ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : RouteRepeatType.NoRepeat;
		}
		#endregion

		#region "Properties"

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

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
		public Nullable<System.DateTime> EndDate { get; set; }

		[DataMember]
		public Nullable<double> Distance { get; set; }

		[DataMember]
		public Nullable<decimal> Charge { get; set; }

		[DataMember]
		public Nullable<int> SeatCount { get; set; }

		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public Nullable<System.DateTime> CreateDate { get; set; }

		[DataMember]
		public Nullable<System.DateTime> UpdateDate { get; set; }
		#endregion

		#region "Properties"
		//[DataMember]
		//public RouteRepeatType RepeatType { get; set; }

		//[DataMember]
		//public Nullable<int> RepeatCondition { get; set; }

		//[DataMember]
		//public Nullable<bool> IsLongTerm { get; set; } 

		[DataMember]
		public RouteType RouteType { get; set; }

		[DataMember]
		public RouteStatus Status { get; set; }
		#endregion

		#region "Properties"
		[IgnoreDataMember]
		public Point From_Point { get { return new Point { Longitude = this.From_Longitude, Latitude = this.From_Latitude }; } set { this.From_Longitude = value.Longitude; this.From_Latitude = value.Latitude; } }

		[IgnoreDataMember]
		public Point To_Point { get { return new Point { Longitude = this.To_Longitude, Latitude = this.To_Latitude }; } set { this.To_Longitude = value.Longitude; this.To_Latitude = value.Latitude; } }
		#endregion

		#region "Properties"
		[IgnoreDataMember]
		public bool IsAvailable { get { return this.Status == RouteStatus.Available; } }

		[IgnoreDataMember]
		public Nullable<bool> IsExpired { get; set; }

		[IgnoreDataMember]
		public bool IsCitywide { get { return ((int)this.RouteType & 0x10) != 0; } }

		[IgnoreDataMember]
		public bool IsIntercity { get { return ((int)this.RouteType & 0x20) != 0; } }
		#endregion

		#endregion
	}

	[DataContract]
	public class RouteParticalView : Route
	{
		#region "Properties"
		[IgnoreDataMember]
		public new long ID { get; set; } 
		#endregion
	}
}
