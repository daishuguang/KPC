using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public abstract class UserTrackRequest
	{
		#region "Properties"
		[DataMember]
		public UserTrackRequestFilter Filter { get; set; }
		#endregion

		#region "Internal Class"
		[DataContract]
		public class UserTrackRequestFilter
		{
			#region "Properties"
			[DataMember]
			public Guid UserID { get; set; }

			[DataMember]
			public Nullable<UserRole> UserRole { get; set; }

			[DataMember]
			public Nullable<Range> Range { get; set; }
			#endregion

			#region "Methods"
			public override string ToString()
			{
				return string.Format("UserID: [{0}], UserRole: {1}, Range: {2}", this.UserID, this.UserRole.HasValue ? this.UserRole.Value.ToString() : "All", this.Range.HasValue ? this.Range.Value.ToString() : "All");
			} 
			#endregion
		} 
		#endregion
	}

	[DataContract]
	public class UserTrackRequestList : UserTrackRequest
	{
		#region "Properties"
		[DataMember]
		public Nullable<int> Page { get; set; }

		[DataMember]
		public Nullable<int> Count { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Filter: [{0}], Page: {1}, Count: {2}", this.Filter.ToString(), this.Page ?? 0, this.Count ?? 10);
		} 
		#endregion
	}

	[DataContract]
	public class UserTrackRequestMap : UserTrackRequest
	{
		#region "Properties"
		[DataMember]
		public Nullable<int> Count { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Filter: [{0}], Count: {1}", this.Filter.ToString(), this.Count ?? 10);
		} 
		#endregion
	}
}
