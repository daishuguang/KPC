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
	public class UserInsurance
	{
		#region "Constructs"
		public UserInsurance() { }

		public UserInsurance(DataStores.tbl_User_Insurance entity)
		{
			if (entity != null)
			{
				Utils.CommonUtility.CopyTo(entity, this);

				this.InsuranceCompany = (InsuranceCompanyType)Enum.Parse(typeof(InsuranceCompanyType), entity.ICID.ToString());
				this.Status = (entity.Status != null && entity.Status.HasValue) ? ((InsuranceStatus)Enum.Parse(typeof(InsuranceStatus), entity.Status.Value.ToString())) : (InsuranceStatus.Pending);
			}
		}
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
		public string InsureKey { get; set; }

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
		public string OccupationCode { get; set; }

		[DataMember]
		public string ActivityCode { get; set; }

		[DataMember]
		public string PresentCode { get; set; }

		[DataMember]
		public string TSRCode { get; set; }

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
		[IgnoreDataMember]
		public bool IsExpired { get { return (this.ExpireDate == null || !this.ExpireDate.HasValue || DateTime.Now.CompareTo(this.ExpireDate.Value) > 0); } }

		[IgnoreDataMember]
		public bool IsAvailable { get { return (this.Status == null || this.Status.Value == InsuranceStatus.Active || this.Status.Value == InsuranceStatus.Pending); } }
		#endregion
	}
}
