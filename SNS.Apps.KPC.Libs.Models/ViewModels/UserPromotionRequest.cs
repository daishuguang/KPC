using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserPromotionRequest : ViewModels.PagedRequest
	{
		#region "Properties"
		[DataMember]
		public UserPromotionRequestFilter Filter { get; set; } 
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Filter: {0}, Page: {1}, Count: {2}", this.Filter, this.Page ?? 0, this.Count ?? 10);
		} 
		#endregion

		#region "Internal Class"
		[DataContract]
		public class UserPromotionRequestFilter
		{
			#region "Properties"
			[DataMember]
			public Nullable<Guid> UserID { get; set; }

			[DataMember]
			public UserPromotionType PromotionType { get; set; }
			#endregion

			#region "Methods"
			public override string ToString()
			{
				return string.Format("UserID: {0}, PromotionType: {1}", this.UserID != null && this.UserID.HasValue ? this.UserID.Value.ToString() : String.Empty, this.PromotionType);
			}
			#endregion
		} 
		#endregion
	}
}
