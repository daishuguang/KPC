using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SNS.Apps.KPC.Libs.Models
{
	public class InsuranceSubmitRequest
	{
		#region "Properties"
		[DataMember]
		public long RequestorID { get; set; }

		[DataMember]
		public InsuranceSubmitModel Request { get; set; }
		#endregion
	}

	[DataContract]
	public class InsuranceSubmitRequest<T> : InsuranceSubmitRequest
		where T : InsuranceSubmitRequestExtend
	{
		#region "Constructs"
		public InsuranceSubmitRequest()
		{
			if (this.RequestExtend == null)
			{
				this.RequestExtend = Activator.CreateInstance<T>();
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public T RequestExtend { get; set; } 
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("RequestorID: {0}, Request: [{1}]", this.RequestorID, this.Request);
		}
		#endregion
	}

	[DataContract]
	public abstract class InsuranceSubmitRequestExtend
	{
		#region "Properties"
		[IgnoreDataMember]
		public abstract InsuranceCompanyType InsuranceCompany { get; }

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
	}

	[DataContract]
	public class InsuranceSubmitRequestExtend_Metlife : InsuranceSubmitRequestExtend
	{
		#region "Properties"
		[IgnoreDataMember]
		public override InsuranceCompanyType InsuranceCompany
		{
			get
			{
				return InsuranceCompanyType.MetLife;
			}
		}

		[DataMember]
		public string SystemName { get; set; }

		[DataMember]
		public string InsureKey { get; set; }

		[DataMember]
		public string InsureNo { get; set; }

		[DataMember]
		public string OccupationCode { get; set; }

		[DataMember]
		public string ActivityCode { get; set; }

		[DataMember]
		public string PresentCode { get; set; }

		[DataMember]
		public string TSRCode { get; set; }
		#endregion
	}
}
