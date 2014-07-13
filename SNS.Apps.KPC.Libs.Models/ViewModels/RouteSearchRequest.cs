using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
    [DataContract]
    public class RouteSearchRequest : ViewModels.PagedRequest
    {
        #region "Properties"
        [DataMember]
        public RouteSearchFilter Filter { get; set; }
        #endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Filter: [{0}], Page: {1}, Count: {2}", this.Filter.ToString(), (this.Page.HasValue ? this.Page.Value.ToString() : "0"), (this.Count.HasValue ? this.Count.Value.ToString() : "10"));
		} 
		#endregion
    }

    [DataContract]
    public class RouteSearchFilter
    {
		#region "Properties"
		[DataMember]
		public RouteSearch_LocationFilter LocationFilter { get; set; }

		[DataMember]
		public RouteSearch_DateFilter DateFilter { get; set; }

		[DataMember]
		public Nullable<long> UserID { get; set; }

		[DataMember]
		public Nullable<UserRole> UserRole { get; set; }

		[DataMember]
		public Nullable<bool> ExcludeSelf { get; set; }
		#endregion

		#region "Methods"
		public override int GetHashCode()
		{
			var sb = new System.Text.StringBuilder();

			#region "Build Identity String"
			// From
			if (this.LocationFilter.From_Point.HasValue)
			{
				sb.AppendFormat("{0}_", this.LocationFilter.From_Point.Value.ToSerialize());
			}
			else
			{
				sb.Append("0.0_0.0_");
			}
			/* End */

			// To
			if (this.LocationFilter.To_Point.HasValue)
			{
				sb.AppendFormat("{0}_", this.LocationFilter.To_Point.Value.ToSerialize());
			}
			else
			{
				sb.Append("0.0_0.0_");
			}
			/* End */

			sb.AppendFormat("{0}_", (this.UserID != null && this.UserID.HasValue ? this.UserID.Value.ToString() : "0"));
			sb.AppendFormat("{0}_", (this.UserRole != null && this.UserRole.HasValue ? this.UserRole.Value.ToString() : "All"));
			sb.AppendFormat("{0}_", (this.ExcludeSelf != null && this.ExcludeSelf.HasValue ? this.ExcludeSelf.Value.ToString() : "False"));

			// Date
			if (this.DateFilter.Date.HasValue)
			{
				sb.AppendFormat("{0}_", this.DateFilter.Date.Value.ToString("yyyyMMdd"));
			}
			else
			{
				sb.Append("All_");
			}

			sb.Append(this.DateFilter.Range.ToString()); 
			/* End */
			#endregion

			return sb.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("LocationFilter: [{0}], DateFilter: [{1}], UserID: {2}, UserRole: {3}, ExcludeSelf: {4}", 
				(this.LocationFilter != null) ? (this.LocationFilter.ToString()) : ("NULL"), 
				(this.DateFilter != null) ? (this.DateFilter.ToString()) : ("NULL"), 
				(this.UserID != null && this.UserID.HasValue ? this.UserID.Value.ToString() : "0"),
				(this.UserRole != null && this.UserRole.HasValue ? this.UserRole.Value.ToString() : "All"), 
				(this.ExcludeSelf != null && this.ExcludeSelf.HasValue ? this.ExcludeSelf.Value.ToString() : "False")
			);
		}
		#endregion
	}

	[DataContract]
	public class RouteSearch_LocationFilter
	{
		#region "Properties"
		#region "Location: From"
		[DataMember]
		public string From_Province { get; set; }

		[DataMember]
		public string From_City { get; set; }

		[DataMember]
		public string From_District { get; set; }

		[DataMember]
		public string From_Location { get; set; }

		[DataMember]
		public Nullable<Point> From_Point { get; set; }
		#endregion

		#region "Location: To"
		[DataMember]
		public string To_Province { get; set; }

		[DataMember]
		public string To_City { get; set; }

		[DataMember]
		public string To_District { get; set; }

		[DataMember]
		public string To_Location { get; set; }

		[DataMember]
		public Nullable<Point> To_Point { get; set; }
		#endregion 

		[DataMember]
		public RouteType RouteType { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendFormat("From_Province: {0}, ", this.From_Province);
			sb.AppendFormat("From_City: {0}, ", this.From_City);
			sb.AppendFormat("From_District: {0}, ", this.From_District);
			sb.AppendFormat("From_Location: {0}, ", this.From_Location);
			sb.AppendFormat("From_Point: {0}, ", this.From_Point.HasValue ? this.From_Point.Value.ToString() : "[0, 0]");

			sb.AppendFormat("To_Province: {0}, ", this.To_Province);
			sb.AppendFormat("To_City: {0}, ", this.To_City);
			sb.AppendFormat("To_District: {0}, ", this.To_District);
			sb.AppendFormat("To_Location: {0}, ", this.To_Location);
			sb.AppendFormat("To_Point: {0}, ", this.To_Point.HasValue ? this.To_Point.Value.ToString() : "[0, 0]");

			return sb.ToString().TrimEnd(' ', ',');
		} 
		#endregion
	}

	[DataContract]
	public class RouteSearch_DateFilter
	{
		#region "Properties"
		[DataMember]
		public Nullable<DateTime> Date { get; set; }

		[DataMember]
		public int Range { get; set; } 
		#endregion

		#region "Methods"
		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendFormat("Date: {0}, Range: {1}", this.Date.HasValue ? this.Date.Value.ToString("yyyy-MM-dd") : "NULL", this.Range);

			return sb.ToString();
		} 
		#endregion
	}
}