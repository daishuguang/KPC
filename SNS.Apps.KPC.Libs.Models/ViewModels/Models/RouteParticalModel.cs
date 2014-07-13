using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
    [DataContract]
    public class RouteParticalModel
    {
        #region "Constructs"
        public RouteParticalModel() { }

        public RouteParticalModel(Route entity)
        {
            //this.RouteID = entity.ID;
            this.RouteGUID = entity.RouteGUID;

            #region "From_Location"
            this.From_Province = entity.From_Province;
            this.From_City = entity.From_City;
            this.From_District = entity.From_District;
            this.From_Location = entity.From_Location;
            this.From_Point = entity.From_Point;
            #endregion

            #region "To_Location"
            this.To_Province = entity.To_Province;
            this.To_City = entity.To_City;
            this.To_District = entity.To_District;
            this.To_Location = entity.To_Location;
            this.To_Point = entity.To_Point;
            #endregion

            this.StartDate = entity.StartDate;
            this.SeatCount = entity.SeatCount;
            this.Charge = entity.Charge;
			this.Note = entity.Note;
            this.Status = entity.Status;
            this.RouteType = entity.RouteType;

			this.IsExpired = entity.IsExpired;
			//this.IsLongTerm = entity.IsLongTerm;

            //this.RepeatCondition = (entity.RepeatCondition.HasValue) ? (entity.RepeatCondition.Value) : ((int)RouteRepeatCondition.Repeat_None);
			//this.RepeatType = entity.RepeatType;
        }

		public RouteParticalModel(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			//this.RouteID = entity.ID;
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
			this.IsLongTerm = entity.Route_IsLongTerm;

			//this.RepeatCondition = (entity.RepeatCondition.HasValue) ? (entity.RepeatCondition.Value) : ((int)RouteRepeatCondition.Repeat_None);
			//this.RepeatType = entity.RepeatType.HasValue ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : RouteRepeatType.NoRepeat;
		}

		public RouteParticalModel(DataStores.sp_Load_UserRoute_Newest_Disp_Result entity)
		{
			//this.RouteID = entity.ID;
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
			this.IsLongTerm = entity.Route_IsLongTerm;

			//this.RepeatCondition = (entity.RepeatCondition.HasValue) ? (entity.RepeatCondition.Value) : ((int)RouteRepeatCondition.Repeat_None);
			//this.RepeatType = entity.RepeatType.HasValue ? ((RouteRepeatType)Enum.Parse(typeof(RouteRepeatType), entity.RepeatType.Value.ToString())) : RouteRepeatType.NoRepeat;
		}
        #endregion

        #region "Properties"
		//[DataMember]
		//public long RouteID { get; set; }

        [DataMember]
        public Guid RouteGUID { get; set; }

        #region "From Location"
        [DataMember]
        public string From_Province { get; set; }

        [DataMember]
        public string From_City { get; set; }

        [DataMember]
        public string From_District { get; set; }

        //public string From_Street { get; set; }

        [DataMember]
        public string From_Location { get; set; }
        #endregion

        #region "To Location"
        [DataMember]
        public string To_Province { get; set; }

        [DataMember]
        public string To_City { get; set; }

        [DataMember]
        public string To_District { get; set; }

        //public string To_Street { get; set; }

        [DataMember]
        public string To_Location { get; set; }
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
        #endregion

		#region "Properties"
		[DataMember]
		public Nullable<bool> IsExpired { get; set; }

		[DataMember]
		public Nullable<bool> IsLongTerm { get; set; } 
		#endregion

        #region "Properties"
		//[DataMember]
		//public RouteRepeatType RepeatType { get; set; }

		//[DataMember]
		//public Nullable<int> RepeatCondition { get; set; }

        [DataMember]
        public RouteType RouteType { get; set; }

        [DataMember]
        public RouteStatus Status { get; set; }
        #endregion

        #region "Properties"
        [DataMember]
        public Point From_Point { get; set; }

        [DataMember]
        public Point To_Point { get; set; }
        #endregion

		#region "Properties"
		[IgnoreDataMember]
		public bool IsAvailable { get { return this.Status == RouteStatus.Available; } } 
		#endregion
    }
}
