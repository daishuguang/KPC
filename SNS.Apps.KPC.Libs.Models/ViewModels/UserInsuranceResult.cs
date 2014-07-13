using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserInsuranceResult
	{
		#region "Constructs"
		public UserInsuranceResult() { }

		public UserInsuranceResult(UserInsurance entity) { if (entity != null) { Utils.CommonUtility.CopyTo(entity, this); } }
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string Folio { get; set; }

		[DataMember]
		public UserPrivy Requestor { get; set; }

		[DataMember]
		public Nullable<InsuranceCompanyType> InsuranceCompany { get; set; }

		[DataMember]
		public string InsureNo { get; set; }

		[DataMember]
		public string Province { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string District { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public Nullable<DateTime> IssueDate { get; set; }

		[DataMember]
		public string IssueMemo { get; set; }

		[DataMember]
		public Nullable<DateTime> EffectDate { get; set; }

		[DataMember]
		public Nullable<DateTime> ExpireDate { get; set; }

		[DataMember]
		public Nullable<InsuranceStatus> Status { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public bool IsExpired { get; set; }

		[DataMember]
		public bool IsAvailable { get; set; }
		#endregion
	}
}
